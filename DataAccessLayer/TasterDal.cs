
using ItbApi.Helpers;
using ItbApi.Models;
using ItbApi.Models.RequestModels;
using ItbApi.Models.ViewModels;
using ItbApi.TaterBusiness;
using Microsoft.EntityFrameworkCore;
using ReviewerAPI.BeerHubs;

namespace ItbApi.DataAccessLayer
{
    public class TasterDal(
        ItbContext context,
        IUnTapped unTapped,
        INotificationService notificationService) : ITasterDal
    {
        public ItbContext Context { get; } = context;
        public IUnTapped UnTapped { get; } = unTapped;
        public INotificationService NotificationService { get; } = notificationService;

        public async Task<Result> CreateUser(User user)
        {
            try
            {
                await Context.Users.AddAsync(user);
                await Context.SaveChangesAsync();
                if (user.Room == null)
                    return new Result(ResultStatus.SUCCESS, "User created");
                else
                    return new Result(ResultStatus.SUCCESS, user.Room.Code);

            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, "Error creating user! " + ex.Message);
            }
        }

        public async Task<JoinRoomResponse> JoinRoom(JoinRoomRequest request)
        {
            try
            {
                Room roomToJoin = Context.Rooms.IncludeAll().FirstOrDefault(a => a.Code == request.roomCode);
                if (roomToJoin == null)
                    return new JoinRoomResponse(ResultStatus.ERROR, "Room code or pin is incorrect");

                if (roomToJoin.Pin != request.pin)
                    return new JoinRoomResponse(ResultStatus.ERROR, "Room code or pin is incorrect");

                if (roomToJoin.Users.Any(a => a.Name == request.username))
                    return new JoinRoomResponse(ResultStatus.ERROR, "Username taken");


                Guid userId = Guid.NewGuid();

                await Context.Users.AddAsync(new User()
                {
                    Id = userId,
                    DateCreated = DateTime.Now,
                    IsOwner = false,
                    Name = request.username,
                    RoomId = roomToJoin.Id,
                });

                await Context.SaveChangesAsync();

                await NotificationService.NewUserJoined(request.roomCode, request.username, userId);
                return new JoinRoomResponse(userId);
            }
            catch (Exception ex)
            {
                return new JoinRoomResponse(ResultStatus.ERROR, ex.Message);
            }

        }

        public async Task<CreateRoomResult> CreateRoom(Room roomToAdd)
        {
            try
            {
                await Context.Rooms.AddAsync(roomToAdd);
                await Context.SaveChangesAsync();
                return new CreateRoomResult(roomToAdd.Code);
            }
            catch (Exception ex)
            {
                return new CreateRoomResult(ResultStatus.ERROR, "Error occurred.");
            }
        }

        public async Task<bool> IsRoomCodeTaken(string roomCode)
        {
            return await Context.Rooms.AnyAsync(a => a.Code == roomCode);
        }

        public async Task<Room> GetRoomByCodeAndPin(string code, int pin)
        {
            return await Context.Rooms.IncludeAll().FirstOrDefaultAsync(a => a.Code == code && a.Pin == pin);
        }

        public async Task<List<Beverage>> SearchForBeers(string query)
        {
            return await Context.Beverages.IncludeAll().Where(a => a.Name.Contains(query) || a.Brewery.Name.Contains(query)).ToListAsync();
        }

        public async Task<List<Beverage>> GetBeveragesByIds(IEnumerable<int> beverageIds)
        {
            return await Context.Beverages.IncludeAll().Where(a => beverageIds.Contains(a.Id)).ToListAsync();
        }

        public async Task<Result> FinishRoomCreation(FinishRoomSettings.Root request)
        {
            try
            {
                Room room = await Context.Rooms.IncludeAll().FirstOrDefaultAsync(a => a.Code == request.roomCode);
                if (room == null)
                    return new Result(ResultStatus.ERROR, "Room not found!");

                if (room.Pin != request.pin)
                    return new Result(ResultStatus.ERROR, "Unautorized");

                if (request.categories.Sum(a => a.maxValue) != 100)
                    return new Result(ResultStatus.ERROR, "Review types sum has to be 100");

                if (request.categories.Any(a => a.isNew && (!string.IsNullOrEmpty(a.name) || !string.IsNullOrEmpty(a.abbr))))
                    return new Result(ResultStatus.ERROR, "Your added categories are missing name or abbriviation");


                List<Beverage> beverages = await GetBeveragesByIds(request.beers.Select(a => a.id));

                int index = 0;
                var beersToRetry = new List<FinishRoomSettings.Beer>();
                foreach (var beverage in request.beers)
                {
                    if (!beverages.Any(a => a.Id == beverage.id))
                    {
                        UnTappedBeerInfo.Root info = await UnTapped.GetBeerInfo(beverage.id);

                        if (info == null)
                        {
                            beersToRetry.Add(beverage);
                            continue;
                        }

                        await this.AddUnTappedBeer(info);
                    }

                    room.Room2Beverages.Add(new Room2Beverage()
                    {
                        BeverageId = beverage.id,
                        DisplayOrder = index,
                        FinalScore = 0,
                        IsLocked = true
                    });

                    index++;
                }

                List<ReviewType> reviewtypes = Context.ReviewTypes.IncludeAll().Where(a => request.categories.Select(b => b.id).Contains(a.Id)).ToList();

                index = 0;
                foreach (var type in request.categories)
                {
                    if (!reviewtypes.Any(a => a.Id == type.id))
                    {
                        AddReviewType(new AddNewReviewType()
                        {
                            name = type.name,
                            ipAddress = request.ipAddress,
                            abbr = type.abbr
                        });
                        reviewtypes.Add(Context.ReviewTypes.FirstOrDefault(a => a.Name == type.name));
                    }

                    room.Room2ReviewTypes.Add(new Room2ReviewType
                    {
                        DisplayOrder = index,
                        MaxValue = type.maxValue,
                        ReviewType = reviewtypes.FirstOrDefault(a => a.Name == type.name)
                    });
                    index++;
                }

                await Context.SaveChangesAsync();
                return new Result(ResultStatus.SUCCESS, "Room creation finished");
            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, ex.Message);
            }
        }

        public async Task<ReviewResults> FinishReview(string roomCode, int beerId)
        {
            Room room = await Context.Rooms.IncludeAll().FirstOrDefaultAsync(a => a.Code == roomCode);
            Room2Beverage beer = room.Room2Beverages.FirstOrDefault(a => a.BeverageId == beerId);
            List<BeverageReview> reviews = room.BeverageReviews.Where(a => a.BeverageId == beerId).ToList();
            if (!reviews.Any())
            {
                return new ReviewResults(ResultStatus.ERROR, "No reviews has been added!");
            }

            beer.ReviewFinished = true;
            List<ReviewTypeVM> reviewTypes = room.Room2ReviewTypes.Select(a => new ReviewTypeVM(a)).ToList();

            await Context.SaveChangesAsync();
            ReviewResults result = new ReviewResults(reviews, reviewTypes);
            await NotificationService.PushFinalScore(roomCode, beerId, result.totalScore);
            return result;
        }

        public async Task<Result> UnlockBeer(string roomCode, int beerId)
        {
            Room room = await Context.Rooms.IncludeBeerAll().FirstOrDefaultAsync(a => a.Code == roomCode);
            if (room == null)
                return new Result(ResultStatus.ERROR, "Room not found");

            List<Room2Beverage> beers = room.Room2Beverages.ToList();
            if (!beers.Any(a => a.BeverageId == beerId))
                return new Result(ResultStatus.ERROR, "Beer not found");

            foreach (var b in beers)
            {
                if (b.BeverageId == beerId)
                    b.IsLocked = false;
                else
                    b.IsLocked = true;
            }

            await Context.SaveChangesAsync();
            await NotificationService.OpenBeerForUser(roomCode, beerId);
            return new Result(ResultStatus.SUCCESS, "Beer unlocked");
        }

        public async Task<Result> AddBeverage(AddNewBeverage request)
        {
            try
            {
                await Context.Beverages.AddAsync(new Beverage()
                {
                    AlcoholPercentage = request.alcaholPercent,
                    DateCreated = DateTime.Now,
                    ImageUrlSm = request.imageUrl,
                    Name = request.name,
                    MajorGroup = request.majorGroupId,
                    Type = request.type,
                    IpAddress = request.ipAddress,
                    Accepted = false
                }
                );

                await Context.SaveChangesAsync();
                return new Result(ResultStatus.SUCCESS, "Beverage created");
            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, ex.Message);

            }
        }

        public async Task<Result> AddReviewType(AddNewReviewType request)
        {
            try
            {
                await Context.ReviewTypes.AddAsync(new ReviewType()
                {
                    Accepted = false,
                    DateAdded = DateTime.Now,
                    IpAddress = request.ipAddress,
                    Name = request.name,
                    Abbr = request.abbr
                });

                await Context.SaveChangesAsync();
                return new Result(ResultStatus.SUCCESS, "Review type created");
            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, ex.Message);
            }
        }

        public async Task<Dashboard> GetDashboard(string roomCode, int pin)
        {
            try
            {
                Room room = await Context.Rooms.IncludeAll().FirstOrDefaultAsync(a => a.Code.ToLower() == roomCode.ToLower() && a.Pin == pin);
                if (room == null)
                    return new Dashboard(ResultStatus.ERROR, "Room code or pin is incorrect");


                return new Dashboard(room);
            }
            catch (Exception ex)
            {
                return new Dashboard(ResultStatus.ERROR, ex.Message);
            }
        }

        public async Task<UserRoomModel> GetRoom(string roomCode, int pin, Guid userId)
        {
            try
            {
                Room room = await Context.Rooms.IncludeAll().FirstOrDefaultAsync(a => a.Code == roomCode && a.Pin == pin);
                if (room == null)
                    return new UserRoomModel(ResultStatus.ERROR, "Room code or pin is incorrect");

                User user = await Context.Users.IncludeAll().FirstOrDefaultAsync(a => a.Id == userId);

                return new UserRoomModel(room, user);
            }
            catch (Exception ex)
            {
                return new UserRoomModel(ResultStatus.ERROR, ex.Message);
            }
        }

        public async Task<Result> AddReview(AddReviewRequest reqiest)
        {
            try
            {
                User user = await Context.Users.IncludeAll().FirstOrDefaultAsync(a => a.Id == reqiest.userId);
                Room room = user.Room;

                if (!room.Room2Beverages.Any(a => a.BeverageId == reqiest.beverageId))
                    return new Result(ResultStatus.ERROR, "Beverage not in current room");

                if (room.Room2Beverages.Any(a => a.BeverageId == reqiest.beverageId && a.IsLocked))
                    return new Result(ResultStatus.ERROR, "Beverage closed for reviewing");

                if (reqiest.parts.Any(a => a.score < 0))
                    return new Result(ResultStatus.ERROR, "Score cannot be negative");

                string resultmessage = "";
                BeverageReview review = new BeverageReview();
                decimal total = 0;
                foreach (var part in reqiest.parts)
                {
                    var reviewType = room.Room2ReviewTypes.FirstOrDefault(a => a.ReviewTypeId == part.reviewTypeId);
                    total += (reviewType.MaxValue / 100m) * part.score;
                }

                total = Math.Round(total, 2);

                if (!user.BeverageReviews.Any(a => a.BeverageId == reqiest.beverageId))
                {
                    List<ReviewPart> partsToAdd = new List<ReviewPart>();
                    partsToAdd.AddRange(reqiest.parts.Select(a => new ReviewPart()
                    {
                        ReviewTypeId = a.reviewTypeId,
                        Score = a.score
                    }));

                    review = new BeverageReview()
                    {
                        BeverageId = reqiest.beverageId,
                        DateCreated = DateTime.Now,
                        RoomId = room.Id,
                        TotalScore = total,
                        ReviewParts = partsToAdd,
                        Comment = reqiest.comment
                    };
                    user.BeverageReviews.Add(review);

                    resultmessage = "Review Added";
                }
                else
                {
                    review = user.BeverageReviews.FirstOrDefault(a => a.BeverageId == reqiest.beverageId);
                    review.TotalScore = total;
                    review.Comment = reqiest.comment;


                    foreach (var part in reqiest.parts)
                    {
                        ReviewPart p = review.ReviewParts.FirstOrDefault(a => a.ReviewTypeId == part.reviewTypeId);
                        p.Score = part.score;
                    }

                    resultmessage = "Review Updated";
                }

                await Context.SaveChangesAsync();
                Beverage b = room.Room2Beverages.FirstOrDefault(a => a.BeverageId == reqiest.beverageId).Beverage;
                List<ReviewTypeVM> types = room.Room2ReviewTypes.Select(a => new ReviewTypeVM(a)).ToList();
                await NotificationService.PushReview(room.Code, new BeverageReviewVM(b, types, user, review));

                return new Result(ResultStatus.SUCCESS, resultmessage);
            }
            catch (Exception ex)
            {
                return new UserRoomModel(ResultStatus.ERROR, ex.Message);
            }

        }

        public async Task<List<BeverageType>> GetBeverageTypes()
        {
            List<BeverageType> types = await Context.BeverageTypes.ToListAsync();
            return types;
        }

        public async Task<List<Brewery>> GetAllBreweries()
        {
            List<Brewery> breweries = await Context.Breweries.ToListAsync();
            return breweries;
        }

        public async Task AddUnTappedBeers(UnTappedBeerSearch.Root info)
        {
            var beers = info.response.beers;

            HashSet<int> bIds = Context.Beverages.Select(a => a.Id).ToHashSet();
            List<BeverageType> types = await GetBeverageTypes();
            List<BeverageType> typeToAdd = new List<BeverageType>();

            foreach (var beerstyle in info.response.beers.items.GroupBy(a => a.beer.beer_style))
            {
                if (!types.Any(a => a.Name == beerstyle.First().beer.beer_style))
                {
                    var type = new BeverageType()
                    {
                        Name = beerstyle.First().beer.beer_style,
                        DisplayOrder = 0
                    };
                    types.Add(type);
                    typeToAdd.Add(type);
                }
            }

            await this.Context.BeverageTypes.AddRangeAsync(typeToAdd);
            await this.Context.SaveChangesAsync();

            types = await GetBeverageTypes();
            List<Brewery> breweries = await GetAllBreweries();
            List<Brewery> breweriesToAdd = new List<Brewery>();

            foreach (var b in info.response.beers.items.Select(a => a.brewery).GroupBy(a => a.brewery_id))
            {
                var brewery = b.First();
                Brewery breweryToAdd = new Brewery();
                if (!breweries.Any(a => a.Id == brewery.brewery_id))
                {
                    breweryToAdd = new Brewery()
                    {
                        City = brewery.location.brewery_city,
                        Country = brewery.country_name,
                        DateAdded = DateTime.Now,
                        Facebook = string.Empty,
                        Id = brewery.brewery_id,
                        Logo = brewery.brewery_label,
                        Name = brewery.brewery_name,
                        Twitter = string.Empty,
                        WebPage = string.Empty
                    };
                    breweries.Add(breweryToAdd);
                    breweriesToAdd.Add(breweryToAdd);
                }
            }

            await this.Context.Breweries.AddRangeAsync(breweriesToAdd);
            await this.Context.SaveChangesAsync();

            breweries = await GetAllBreweries();


            int typeId = -1;
            List<Beverage> toAdd = new List<Beverage>();
            foreach (var beer in beers.items)
            {
                if (!bIds.Any(a => a == beer.beer.bid))
                {
                    Beverage beverageToAdd = new Beverage()
                    {
                        Accepted = true,
                        AlcoholPercentage = Convert.ToDecimal(beer.beer.beer_abv),
                        MajorGroup = 1,
                        DateCreated = DateTime.Now,
                        Description = beer.beer.beer_description,
                        IBU = beer.beer.beer_ibu,
                        Id = beer.beer.bid,
                        IpAddress = "",
                        LableMed = beer.beer.beer_label,
                        LableSm = beer.beer.beer_label,
                        ImageUrlSm = beer.beer.beer_label,
                        ImageUrlMed = beer.beer.beer_label,
                        Name = beer.beer.beer_name,
                        UtappedRating = 0,
                        Type = types.FirstOrDefault(a => a.Name == beer.beer.beer_style).Id,
                        BreweryId = beer.brewery.brewery_id
                    };

                    toAdd.Add(beverageToAdd);
                }
            }

            if (toAdd.Any())
            {
                await this.Context.Beverages.AddRangeAsync(toAdd);
                await Context.SaveChangesAsync();
            }
        }

        public async Task AddUnTappedBeer(UnTappedBeerInfo.Root info)
        {
            UnTappedBeerInfo.Beer beer = info.response.beer;
            List<BeverageType> types = await GetBeverageTypes();
            BeverageType typeToAdd = new BeverageType();
            int typeId = -1;
            if (!types.Any(a => a.Name == beer.beer_style))
            {
                typeToAdd = new BeverageType()
                {
                    Name = beer.beer_style,
                    DisplayOrder = 0
                };
            }
            else
            {
                typeId = types.FirstOrDefault(a => a.Name == beer.beer_style).Id;
            }

            int breweryId = -1;
            List<Brewery> breweries = await GetAllBreweries();
            Brewery breweryToAdd = new Brewery();
            if (!breweries.Any(a => a.Id == beer.brewery.brewery_id))
            {
                breweryToAdd = new Brewery()
                {
                    City = beer.brewery.location.brewery_city,
                    Country = beer.brewery.country_name,
                    DateAdded = DateTime.Now,
                    Facebook = beer.brewery.contact.facebook,
                    Id = beer.brewery.brewery_id,
                    Logo = beer.brewery.brewery_label,
                    Name = beer.brewery.brewery_name,
                    Twitter = beer.brewery.contact.twitter,
                    WebPage = beer.brewery.contact.url
                };
            }
            else
            {
                breweryId = beer.brewery.brewery_id;
            }

            if (!Context.Beverages.Any(a => a.Id == beer.bid))
            {
                Beverage beverageToAdd = new Beverage()
                {
                    Accepted = true,
                    AlcoholPercentage = Convert.ToDecimal(beer.beer_abv),
                    MajorGroup = 1,
                    DateCreated = DateTime.Now,
                    Description = beer.beer_description,
                    IBU = beer.beer_ibu,
                    Id = beer.bid,
                    IpAddress = "",
                    LableMed = beer.beer_label_hd,
                    LableSm = beer.beer_label,
                    Name = beer.beer_name,
                    UtappedRating = Convert.ToDecimal(beer.rating_score)
                };

                if (typeId != -1)
                    beverageToAdd.Type = typeId;
                else
                {
                    beverageToAdd.BeverageType = typeToAdd;
                }

                if (breweryId != -1)
                    beverageToAdd.BreweryId = breweryId;
                else
                {
                    beverageToAdd.Brewery = breweryToAdd;
                }

                if (beer.media.items.Any())
                {
                    beverageToAdd.ImageUrlSm = beer.media.items.FirstOrDefault().photo.photo_img_sm;
                    beverageToAdd.ImageUrlMed = beer.media.items.FirstOrDefault().photo.photo_img_md;
                }
                await Context.Beverages.AddAsync(beverageToAdd);
                await Context.SaveChangesAsync();
            }
        }

        public async Task<List<BeerVM>> GetBreverysBeers(int breweryId)
        {
            List<Beverage> beers = await Context.Beverages.Where(a => a.BreweryId == breweryId).ToListAsync();
            return beers.Select(a => new BeerVM(a)).OrderByDescending(a => a.untappedRating).ToList();
        }

        public async Task<List<ReviewTypeForCreateVM>> GetReviewTypes()
        {
            return await Context.ReviewTypes.Where(a => a.Accepted).OrderBy(a => a.Name).Select(a => new ReviewTypeForCreateVM(a)).ToListAsync();
        }

        public async Task<Result> DeleteUser(DeleteUserRequest request)
        {
            try
            {
                User user = await Context.Users.FirstOrDefaultAsync(a => a.Id == request.userId);
                Context.Users.Remove(user);
                await Context.SaveChangesAsync();
                return new Result(ResultStatus.SUCCESS, "User Deleted");
            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, ex.Message);
            }
        }
    }
}