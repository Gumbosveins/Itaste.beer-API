using ItbApi.Helpers;
using ItbApi.Models;
using ItbApi.Models.RequestModels;
using Microsoft.EntityFrameworkCore;
using ReviewerAPI.BeerHubs;

namespace ItbApi.DataAccessLayer
{
    /// <summary>
    /// Changes to a room after it has been created: beers, order, rating categories and tasters.
    /// Every change is authorized with the room code and pin, and tells connected screens to reload.
    /// </summary>
    public class RoomEditDal(
        ItbContext context,
        ITasterDal tasterDal,
        IUnTapped unTapped,
        INotificationService notificationService) : IRoomEditDal
    {
        private async Task<Room?> FindRoom(EditRoomRequest request)
        {
            string code = (request.roomCode ?? "").ToUpper();
            return await context.Rooms.IncludeAll().FirstOrDefaultAsync(a => a.Code == code && a.Pin == request.pin);
        }

        private static Result NotFound() => new Result(ResultStatus.NOT_FOUND, "Room code or pin is incorrect");

        private async Task<Result> Saved(Room room, string message)
        {
            await context.SaveChangesAsync();
            await notificationService.RoomUpdated(room.Code ?? "");
            return new Result(ResultStatus.SUCCESS, message);
        }

        private static void Renumber(IEnumerable<Room2Beverage> beverages)
        {
            int index = 0;
            foreach (var b in beverages.OrderBy(a => a.DisplayOrder))
            {
                b.DisplayOrder = index++;
            }
        }

        public async Task<Result> AddBeverages(AddBeveragesRequest request)
        {
            Room? room = await FindRoom(request);
            if (room == null)
            {
                return NotFound();
            }

            var newIds = request.beverageIds.Distinct().Where(id => !room.Room2Beverages.Any(a => a.BeverageId == id)).ToList();
            if (!newIds.Any())
            {
                return new Result(ResultStatus.BAD_DATA, "Those beers are already in the room");
            }

            List<Beverage> known = await tasterDal.GetBeveragesByIds(newIds);
            int next = room.Room2Beverages.Any() ? room.Room2Beverages.Max(a => a.DisplayOrder) + 1 : 0;
            var missing = new List<int>();

            foreach (int id in newIds)
            {
                if (!known.Any(a => a.Id == id))
                {
                    var info = await unTapped.GetBeerInfo(id);
                    if (info == null)
                    {
                        missing.Add(id);
                        continue;
                    }
                    await tasterDal.AddUnTappedBeer(info);
                }

                room.Room2Beverages.Add(new Room2Beverage
                {
                    BeverageId = id,
                    DisplayOrder = next++,
                    FinalScore = 0,
                    IsLocked = true
                });
            }

            if (missing.Count == newIds.Count)
            {
                return new Result(ResultStatus.ERROR, "Could not fetch the beer from Untappd, try again");
            }

            return await Saved(room, missing.Any() ? "Some beers could not be added" : "Beers added");
        }

        public async Task<Result> RemoveBeverage(RemoveBeverageRequest request)
        {
            Room? room = await FindRoom(request);
            if (room == null)
            {
                return NotFound();
            }

            Room2Beverage? link = room.Room2Beverages.FirstOrDefault(a => a.BeverageId == request.beverageId);
            if (link == null)
            {
                return new Result(ResultStatus.NOT_FOUND, "Beer not found in this room");
            }

            var reviews = await context.BeverageReviews
                .Where(a => a.RoomId == room.Id && a.BeverageId == request.beverageId)
                .ToListAsync();
            context.BeverageReviews.RemoveRange(reviews);
            context.Room2Beverages.Remove(link);
            Renumber(room.Room2Beverages.Where(a => a != link));

            return await Saved(room, "Beer removed");
        }

        public async Task<Result> ReorderBeverages(ReorderBeveragesRequest request)
        {
            Room? room = await FindRoom(request);
            if (room == null)
            {
                return NotFound();
            }

            var current = room.Room2Beverages.Select(a => a.BeverageId).OrderBy(a => a).ToList();
            var proposed = request.beverageIds.OrderBy(a => a).ToList();
            if (!current.SequenceEqual(proposed))
            {
                return new Result(ResultStatus.BAD_DATA, "The new order must contain every beer in the room exactly once");
            }

            for (int i = 0; i < request.beverageIds.Count; i++)
            {
                room.Room2Beverages.First(a => a.BeverageId == request.beverageIds[i]).DisplayOrder = i;
            }

            return await Saved(room, "Order updated");
        }

        public async Task<Result> UpdateCategories(UpdateCategoriesRequest request, string ipAddress)
        {
            Room? room = await FindRoom(request);
            if (room == null)
            {
                return NotFound();
            }

            if (room.BeverageReviews.Any())
            {
                return new Result(ResultStatus.IMPOSSIBLE, "Categories can not change after the first review");
            }

            if (!request.categories.Any() || request.categories.Sum(a => a.maxValue) != 100)
            {
                return new Result(ResultStatus.BAD_DATA, "Category weights have to add up to 100");
            }

            if (request.categories.Any(a => a.isNew && (string.IsNullOrWhiteSpace(a.name) || string.IsNullOrWhiteSpace(a.abbr))))
            {
                return new Result(ResultStatus.BAD_DATA, "New categories need a name and an abbreviation");
            }

            var ids = request.categories.Where(a => !a.isNew).Select(a => a.id).ToList();
            List<ReviewType> types = await context.ReviewTypes.Where(a => ids.Contains(a.Id)).ToListAsync();

            context.Room2ReviewTypes.RemoveRange(room.Room2ReviewTypes);

            int index = 0;
            foreach (var category in request.categories)
            {
                ReviewType? type = category.isNew ? null : types.FirstOrDefault(a => a.Id == category.id);
                if (type == null)
                {
                    type = new ReviewType
                    {
                        Accepted = false,
                        DateAdded = DateTime.UtcNow,
                        IpAddress = ipAddress,
                        Name = category.name,
                        Abbr = category.abbr
                    };
                    context.ReviewTypes.Add(type);
                }

                room.Room2ReviewTypes.Add(new Room2ReviewType
                {
                    DisplayOrder = index++,
                    MaxValue = category.maxValue,
                    ReviewType = type
                });
            }

            return await Saved(room, "Categories updated");
        }

        public async Task<Result> RemoveUser(RemoveUserRequest request)
        {
            Room? room = await FindRoom(request);
            if (room == null)
            {
                return NotFound();
            }

            User? user = room.Users.FirstOrDefault(a => a.Id == request.userId);
            if (user == null)
            {
                return new Result(ResultStatus.NOT_FOUND, "Taster not found in this room");
            }

            var reviews = await context.BeverageReviews.Where(a => a.UserId == user.Id).ToListAsync();
            context.BeverageReviews.RemoveRange(reviews);
            context.Users.Remove(user);

            var result = await Saved(room, $"{user.Name} removed");
            await notificationService.UserRemoved(room.Code ?? "", user.Id);
            return result;
        }
    }
}
