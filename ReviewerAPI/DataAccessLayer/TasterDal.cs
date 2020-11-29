using ReviewerAPI.BeerHubs;
using ReviewerAPI.Models;
using ReviewerAPI.Models.Connection;
using ReviewerAPI.Models.RequestModels;
using ReviewerAPI.Models.ViewModels;
using ReviewerAPI.TaterBusiness;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ReviewerAPI.DataAccessLayer
{
    public class TasterDal
    {
        private TasterDbDataContext context = new TasterDbDataContext();
        public Result CreateUser(User user)
        {
            try
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
                if(user.Room == null)
                    return new Result(ResultStatus.SUCCESS, "User created");
                else
                    return new Result(ResultStatus.SUCCESS, user.Room.Code);

            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, "Error creating user! " + ex.Message);
            }
        }

        internal JoinRoomResponse JoinRoom(JoinRoomRequest request)
        {
            try
            {
                Room roomToJoin = context.Rooms.FirstOrDefault(a => a.Code == request.roomCode);
                if (roomToJoin == null)
                    return new JoinRoomResponse(ResultStatus.ERROR, "Room code or pin is incorrect");

                if (roomToJoin.Pin != request.pin)
                    return new JoinRoomResponse(ResultStatus.ERROR, "Room code or pin is incorrect");

                if (roomToJoin.Users.Any(a => a.Name == request.username))
                    return new JoinRoomResponse(ResultStatus.ERROR, "Username taken");


                Guid userId = Guid.NewGuid();

                roomToJoin.Users.Add(new User()
                {
                    Id = userId,
                    DateCreated = DateTime.Now,
                    IsOwner = false,
                    Name = request.username
                });

                context.SubmitChanges();
                beerhub hub = new beerhub();
                hub.NewUserJoined(request.roomCode, request.username, userId);
                return new JoinRoomResponse(userId);
            }
            catch (Exception ex)
            {
                return new JoinRoomResponse(ResultStatus.ERROR, ex.Message);
            }

        }

        internal CreateRoomResult CreateRoom(Room roomToAdd)
        {
            try
            {
                context.Rooms.InsertOnSubmit(roomToAdd);
                context.SubmitChanges();
                return new CreateRoomResult(roomToAdd.Code);
            }
            catch (Exception ex)
            {
                var ravenClient = new RavenClient("https://29397ec6e8054d6e887027dfbac91e62@o479485.ingest.sentry.io/5524484");
                ravenClient.Capture(new SentryEvent(ex));
                return new CreateRoomResult(ResultStatus.ERROR, "Error occurred.");
            }
        }

        internal bool IsRoomCodeTaken(string roomCode)
        {
            return context.Rooms.Any(a => a.Code == roomCode);
        }

        internal Room GetRoomByCodeAndPin(string code, int pin)
        {
            return context.Rooms.FirstOrDefault(a => a.Code == code && a.Pin == pin);
        }

        internal List<Beverage> SearchForBeers(string query)
        {
            return context.Beverages.Where(a => a.Name.Contains(query) || a.Brewery.Name.Contains(query)).ToList();
        }

        internal List<Beverage> GetBeveragesByIds(IEnumerable<int> beverageIds)
        {
            return context.Beverages.Where(a => beverageIds.Contains(a.Id)).ToList();
        }

        internal async Task<Result> FinishRoomCreation(FinishRoomSettings.Root request)
        {
            try
            {
                Room room = context.Rooms.FirstOrDefault(a => a.Code == request.roomCode);
                if (room == null)
                    return new Result(ResultStatus.ERROR, "Room not found!");

                if (room.Pin != request.pin)
                    return new Result(ResultStatus.ERROR, "Unautorized");

                if (request.categories.Sum(a => a.maxValue) != 100)
                    return new Result(ResultStatus.ERROR, "Review types sum has to be 100");

                if (request.categories.Any(a => a.isNew && (!string.IsNullOrEmpty(a.name) || !string.IsNullOrEmpty(a.abbr))))
                    return new Result(ResultStatus.ERROR, "Your added categories are missing name or abbriviation");


                List<Beverage> beverages = GetBeveragesByIds(request.beers.Select(a => a.id));

                TasterBusiness business = new TasterBusiness();

                int index = 0;
                foreach (var beverage in request.beers)
                {
                    if (!beverages.Any(a => a.Id == beverage.id))
                    {
                        await business.AddBeer(beverage.id);
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

                List<ReviewType> reviewtypes = context.ReviewTypes.Where(a => request.categories.Select(b => b.id).Contains(a.Id)).ToList();

                index = 0;
                foreach (var type in request.categories)
                {
                    if (!reviewtypes.Any(a => a.Id == type.id))
                    {
                        AddReviewType(new AddNewReviewType(){
                            name = type.name,
                            ipAddress = request.ipAddress,
                            abbr = type.abbr
                        });
                        reviewtypes.Add(context.ReviewTypes.FirstOrDefault(a => a.Name == type.name));
                    }

                    room.Room2ReviewTypes.Add(new Room2ReviewType
                    {
                        DisplayOrder = index,
                        MaxValue = type.maxValue,
                        ReviewType = reviewtypes.FirstOrDefault(a => a.Name == type.name)
                    });
                    index++;
                }

                context.SubmitChanges();
                return new Result(ResultStatus.SUCCESS, "Room creation finished");
            }
            catch (Exception ex)
            {
                var ravenClient = new RavenClient("https://29397ec6e8054d6e887027dfbac91e62@o479485.ingest.sentry.io/5524484");
                ravenClient.Capture(new SentryEvent(ex));
                return new Result(ResultStatus.ERROR, ex.Message);
            }
        }

        internal ReviewResults FinishReview(string roomCode, int beerId)
        {
            Room room = context.Rooms.FirstOrDefault(a => a.Code == roomCode);
            Room2Beverage beer = room.Room2Beverages.FirstOrDefault(a => a.BeverageId == beerId);
            List<BeverageReview> reviews = room.BeverageReviews.Where(a => a.BeverageId == beerId).ToList();
            if (!reviews.Any())
                return new ReviewResults(ResultStatus.ERROR, "No reviews has been added!");
            beer.ReviewFinished = true;
            List<ReviewTypeVM> reviewTypes = room.Room2ReviewTypes.Select(a => new ReviewTypeVM(a)).ToList();

            context.SubmitChanges();
            ReviewResults result = new ReviewResults(reviews, reviewTypes);
            beerhub hub = new beerhub();
            hub.PushFinalScore(roomCode, beerId, result.totalScore);
            return result;
        }

        internal Result UnlockBeer(string roomCode, int beerId)
        {
            Room room = context.Rooms.FirstOrDefault(a => a.Code == roomCode);
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

            context.SubmitChanges();
            beerhub hub = new beerhub();
            hub.OpenBeerForUser(roomCode, beerId);            
            return new Result(ResultStatus.SUCCESS, "Beer unlocked");
        }

        internal Result AddBeverage(AddNewBeverage request)
        {
            try
            {
                context.Beverages.InsertOnSubmit(new Beverage()
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

                context.SubmitChanges();
                return new Result(ResultStatus.SUCCESS, "Beverage created");
            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, ex.Message);
                
            }
        }

        internal Result AddReviewType(AddNewReviewType request)
        {
            try
            {
                context.ReviewTypes.InsertOnSubmit(new ReviewType()
                    {
                        Accepted = false,
                        DateAdded = DateTime.Now,
                        IpAddress = request.ipAddress,
                        Name = request.name,
                        Abbr = request.abbr
                    });

                context.SubmitChanges();
                return new Result(ResultStatus.SUCCESS, "Review type created");
            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, ex.Message);
            }
        }

        internal Dashboard GetDashboard(string roomCode, int pin)
        {
            try
            {
                Room room = context.Rooms.FirstOrDefault(a => a.Code == roomCode && a.Pin == pin);
                if (room == null)
                    return new Dashboard(ResultStatus.ERROR, "Room code or pin is incorrect");


                return new Dashboard(room);
            }
            catch (Exception ex)
            {
                return new Dashboard(ResultStatus.ERROR, ex.Message);
            }
        }

        internal UserRoomModel GetRoom(string roomCode, int pin, Guid userId)
        {
            try
            {
                Room room = context.Rooms.FirstOrDefault(a => a.Code == roomCode && a.Pin == pin);
                if (room == null)
                    return new UserRoomModel(ResultStatus.ERROR, "Room code or pin is incorrect");

                User user = context.Users.FirstOrDefault(a => a.Id == userId);

                return new UserRoomModel(room, user);
            }
            catch (Exception ex)
            {
                return new UserRoomModel(ResultStatus.ERROR, ex.Message);
            }
        }

        internal Result AddReview(AddReviewRequest reqiest)
        {
            try
            {
                User user = context.Users.FirstOrDefault(a => a.Id == reqiest.userId);
                Room room = user.Room;

                if(!room.Room2Beverages.Any(a => a.BeverageId == reqiest.beverageId))
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
                    EntitySet<ReviewPart> partsToAdd = new EntitySet<ReviewPart>();
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

                context.SubmitChanges();
                beerhub hub = new beerhub();
                Beverage b = room.Room2Beverages.FirstOrDefault(a => a.BeverageId == reqiest.beverageId).Beverage;
                List<ReviewTypeVM> types = room.Room2ReviewTypes.Select(a => new ReviewTypeVM(a)).ToList();
                hub.PushReview(room.Code, new BeverageReviewVM(b, types, user, review));


                return new Result(ResultStatus.SUCCESS, resultmessage);
            }
            catch (Exception ex)
            {
                return new UserRoomModel(ResultStatus.ERROR, ex.Message);
            }
            
        }

        internal List<BeverageType> GetBeverageTypes()
        {
            string cacheKey = "GetBeverageTypes";
            if (!Cache.Has(cacheKey))
            {
                List<BeverageType> types = context.BeverageTypes.ToList();
                Cache.Add(types, cacheKey, Cache.CacheDuration.Hour);
                return types;
            }
            return Cache.Get<List<BeverageType>>(cacheKey);
        }

        internal List<Brewery> GetAllBreweries()
        {
            string cacheKey = "GetAllBreweries";
            if (!Cache.Has(cacheKey))
            {
                List<Brewery> breweries = context.Breweries.ToList();
                Cache.Add(breweries, cacheKey, Cache.CacheDuration.Hour);
                return breweries;
            }
            return Cache.Get<List<Brewery>>(cacheKey);
        }

        internal void AddUnTappedBeer(UnTappedBeerInfo.Root info)
        {
            UnTappedBeerInfo.Beer beer = info.response.beer;
            List<BeverageType> types = GetBeverageTypes();
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
            List<Brewery> breweries = GetAllBreweries();
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

            if (!context.Beverages.Any(a => a.Id == beer.bid))
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
                    Cache.InvalidateCacheFor("GetBeverageTypes");
                }

                if (breweryId != -1)
                    beverageToAdd.BreweryId = breweryId;
                else
                {
                    beverageToAdd.Brewery = breweryToAdd;
                    Cache.InvalidateCacheFor("GetAllBreweries");
                }

                if (beer.media.items.Any())
                {
                    beverageToAdd.ImageUrlSm = beer.media.items.FirstOrDefault().photo.photo_img_sm;
                    beverageToAdd.ImageUrlMed = beer.media.items.FirstOrDefault().photo.photo_img_md;
                }
                context.Beverages.InsertOnSubmit(beverageToAdd);
                context.SubmitChanges();
            }
        }

        internal List<BeerVM> GetBreverysBeers(int breweryId)
        {
            List<Beverage> beers = context.Beverages.Where(a => a.BreweryId == breweryId).ToList();
            return beers.Select(a => new BeerVM(a)).OrderByDescending(a => a.untappedRating).ToList();
        }

        internal List<ReviewTypeForCreateVM> GetReviewTypes()
        {
            return context.ReviewTypes.Where(a => a.Accepted).OrderBy(a => a.Name).Select(a => new ReviewTypeForCreateVM(a)).ToList();
        }

        internal Result DeleteUser(DeleteUserRequest request)
        {
            try
            {
                User user = context.Users.FirstOrDefault(a => a.Id == request.userId);
                context.Users.DeleteOnSubmit(user);
                context.SubmitChanges();
                return new Result(ResultStatus.SUCCESS ,"User Deleted");
            }
            catch (Exception ex)
            {
                return new Result(ResultStatus.ERROR, ex.Message);
            }
        }
    }
}