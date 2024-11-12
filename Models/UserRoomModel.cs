namespace ItbApi.Models
{

    public class Dashboard : Result
    {
        public string code { get; set; }
        public string owner { get; set; }
        public string name { get; set; }
        public int pin { get; set; }
        public bool isBlind { get; set; }
        public string blindRevealCode { get; set; }
        public List<BeverageDashboardVM> beverages { get; set; }
        public List<ReviewTypeVM> reviewTypes { get; set; }
        public List<Users> users { get; set; }
        public Dashboard(ResultStatus status, string message) : base(status, message)
        {

        }
        public Dashboard(Room room)
        {
            code = room.Code;
            owner = room.Owner;
            name = room.Name;
            pin = room.Pin;
            isBlind = room.RoomType == 1;
            blindRevealCode = room.BlindRevealCode;
            reviewTypes = new List<ReviewTypeVM>();
            room.Room2ReviewTypes.ToList().ForEach(a => reviewTypes.Add(new ReviewTypeVM(a)));
            beverages = new List<BeverageDashboardVM>();

            List<BeverageReview> usersReviews = room.BeverageReviews.ToList();
            List<User> u = room.Users.ToList();
            users = new List<Users>();
            u.ForEach(a => users.Add(new Users(a)));

            room.Room2Beverages.ToList().ForEach(a => beverages.Add(new BeverageDashboardVM(reviewTypes, a, usersReviews.Where(b => a.BeverageId == b.BeverageId).ToList(), u)));
        }
    }

    public class Users
    {

        public Guid id { get; set; }
        public string username { get; set; }
        public Users()
        {

        }

        public Users(User user)
        {
            id = user.Id;
            username = user.Name;
        }
    }
    public class UserRoomModel : Result
    {
        public string code { get; set; }
        public string username { get; set; }
        public string owner { get; set; }
        public string name { get; set; }
        public bool isBlind { get; set; }
        public List<ReviewTypeVM> reviewTypes { get; set; }
        public List<BeverageModelVM> beverages { get; set; }
        public UserRoomModel(ResultStatus status, string message) : base(status, message)
        {

        }
        public UserRoomModel(Room room, User user) 
        {
            code = room.Code;
            this.username = username;
            owner = room.Owner;
            name = room.Name;
            isBlind = room.RoomType == 1;
            reviewTypes = new List<ReviewTypeVM>();
            List<Room2ReviewType> types = room.Room2ReviewTypes.ToList();
            types.ForEach(a => reviewTypes.Add(new ReviewTypeVM(a)));
            beverages = new List<BeverageModelVM>();
            List<BeverageReview> allReviews =  room.BeverageReviews.ToList();
            List<BeverageReview> usersReviews = user.BeverageReviews.ToList();
            room.Room2Beverages.ToList().ForEach(a => beverages.Add(new BeverageModelVM(a, reviewTypes, allReviews.Where(b => a.BeverageId == b.BeverageId).ToList(), usersReviews.FirstOrDefault(b => a.BeverageId == b.BeverageId))));
        }

    }

    public class BeverageReviewVM
    {
        public long id { get; set; }
        public string username { get; set; }
        public Guid userId { get; set; }
        public int beverageId { get; set; }
        public decimal totalScore { get; set; }
        public List<ReviewPartVM> parts { get; set; }
        public string comment { get; set; }
        public bool includeInCalculations { get; set; }
        public BeverageReviewVM()
        {

        }
        public BeverageReviewVM(Beverage b, List<ReviewTypeVM> types, User user, BeverageReview model)
        
        {
            beverageId = b.Id;

            if (user != null)
            {
                username = user.Name;
                userId = user.Id;
            }
            else
            {
                username = model.User.Name;
                userId = model.User.Id;
            }
            parts = new List<ReviewPartVM>();

            if (model != null)
            {
                includeInCalculations = true;
                id = model.Id;
                totalScore = model.TotalScore;
                model.ReviewParts.ToList().ForEach(a => parts.Add(new ReviewPartVM(a, types.FirstOrDefault(c => c.reviewId == a.ReviewTypeId))));
                comment = model.Comment;
            }
            else
            {
                Random rnd= new Random(Guid.NewGuid().GetHashCode());
                int rndnumber = rnd.Next();
                id = rndnumber;
                includeInCalculations = false;
                totalScore = 0;
                types.ForEach(a => parts.Add(new ReviewPartVM(null, a)));
                comment = "";
            }
            parts = parts.OrderBy(a => a.displayOrder).ToList();
        }

    }

    public class ReviewPartVM
    {
        public long id { get; set; }
        public int displayOrder { get; set; }
        public int reviewTypeId { get; set; }
        public decimal score { get; set; }
        public ReviewPartVM()
        {

        }
        public ReviewPartVM(ReviewPart model, ReviewTypeVM type)
        {
            displayOrder = type.displayOrder;
            reviewTypeId = type.reviewId;
            if(model != null)
            {
                id = model.Id;
                score = model.Score;
            }
        }
    }
    public class BeverageDashboardVM
    {
        public int id { get; set; }
        public int beverageId { get; set; }
        public int displayOrder { get; set; }
        public string name { get; set; }
        public string manufacturer { get; set; }
        public decimal alcoholPercentage { get; set; }
        public string groupName { get; set; }
        public string typeName { get; set; }
        public bool isLocked { get; set; }
        public string desc { get; set; }
        public List<BeverageReviewVM> reviews { get; set; }
        public string imageUrlSm { get; set; }
        public string imageUrlMed { get; set; }
        public decimal IBU { get; set; }
        public bool reviewFinished { get; set; }
        public ReviewResults results { get; set; }
        public BeverageDashboardVM(List<ReviewTypeVM> types, Room2Beverage model, List<BeverageReview> reviewFromUser, List<User> users)
        {
            Beverage b = model.Beverage;
            id = model.Id;
            beverageId = model.BeverageId;
            IBU = b.IBU;
            displayOrder = model.DisplayOrder;
            name = b.Name;
            desc = b.Description;
            manufacturer = b.Brewery.Name;
            alcoholPercentage = b.AlcoholPercentage;
            groupName = b.BeverageGroup.Name;
            typeName = b.BeverageType.Name;
            isLocked = model.IsLocked;
            imageUrlSm = b.LableSm;
            imageUrlMed = b.LableSm;
            if(!String.IsNullOrEmpty(b.LableMed))
                imageUrlMed = b.LableMed;
            reviewFinished = model.ReviewFinished;
            reviews = new List<BeverageReviewVM>();
            users.ForEach(a => reviews.Add(new BeverageReviewVM(b, types, a, reviewFromUser.FirstOrDefault(c => c.UserId == a.Id))));
            results = new ReviewResults();
            if (reviewFinished)
            {
                results = new ReviewResults(reviews, types);
            }
        }
    }

    public class ReviewResults : Result
    {
        public decimal totalScore { get; set; }
        public List<ReviewCategoryResult> partResults { get; set; }
        public ReviewResults(ResultStatus status, string message) : base(status, message)
        {

        }
        public ReviewResults()
        {
            totalScore = 0;
        }
        public ReviewResults(List<BeverageReviewVM> results, List<ReviewTypeVM> types)
        {
            results = results.Where(a => a.includeInCalculations).ToList();
            totalScore = results.Average(a => a.totalScore);
            var grouped = results.SelectMany(a => a.parts).GroupBy(a => a.reviewTypeId).ToList();
            partResults = new List<ReviewCategoryResult>();
            grouped.ForEach(a => partResults.Add(new ReviewCategoryResult(a.ToList(), types)));
        }
        public ReviewResults(List<BeverageReview> results, List<ReviewTypeVM> types)
        {
            totalScore = results.Average(a => a.TotalScore);
            var grouped = results.SelectMany(a => a.ReviewParts).GroupBy(a => a.ReviewTypeId).ToList();
            partResults = new List<ReviewCategoryResult>();
            grouped.ForEach(a => partResults.Add(new ReviewCategoryResult(a.ToList(), types)));
        }
    }

    public class ReviewCategoryResult
    {
        public int reviewTypeId { get; set; }
        public decimal score { get; set; }
        public string name { get; set; }
        public string abbr { get; set; }
        public ReviewCategoryResult(List<ReviewPartVM> parts, List<ReviewTypeVM> types)
        {
            reviewTypeId = parts.FirstOrDefault().reviewTypeId;
            name = types.FirstOrDefault(a => a.reviewId == reviewTypeId).name;
            abbr = types.FirstOrDefault(a => a.reviewId == reviewTypeId).abbr;
            score = parts.Average(a => a.score);
        }

        public ReviewCategoryResult(List<ReviewPart> parts, List<ReviewTypeVM> types)
        {
            reviewTypeId = parts.FirstOrDefault().ReviewTypeId;
            name = types.FirstOrDefault(a => a.reviewId == reviewTypeId).name;
            abbr = types.FirstOrDefault(a => a.reviewId == reviewTypeId).abbr;
            score = parts.Average(a => a.Score);
        }
    }
    public class BeverageModelVM
    {
        public int id { get; set; }
        public int beverageId { get; set; }
        public int displayOrder { get; set; }
        public string name { get; set; }
        public string manufacturer { get; set; }
        public decimal alcoholPercentage { get; set; }
        public string groupName { get; set; }
        public string typeName { get; set; }
        public bool isLocked { get; set; }
        public BeverageReviewVM review { get; set; }
        public string imageUrlSm { get; set; }
        public string imageUrlMed { get; set; }
        public decimal IBU { get; set; }
        public decimal finalScore { get; set; }
        public BeverageModelVM(Room2Beverage model, List<ReviewTypeVM> types, List<BeverageReview> allReviews, BeverageReview reviewFromUser = null)
        {
            Beverage b = model.Beverage;
            id = model.Id;
            beverageId = model.BeverageId;
            IBU = b.IBU;
            displayOrder = model.DisplayOrder;
            name = b.Name;
            manufacturer = b.Brewery.Name;
            alcoholPercentage = b.AlcoholPercentage;
            groupName = b.BeverageGroup.Name;
            typeName = b.BeverageType.Name;
            isLocked = model.IsLocked;
            imageUrlSm = b.LableSm;
            imageUrlMed = b.LableMed;
            
            if (reviewFromUser != null){
                review = new BeverageReviewVM(b, types, null, reviewFromUser);
            }
            else
                review = new BeverageReviewVM();

            if (allReviews.Any())
            {
                finalScore = allReviews.Average(a => a.TotalScore);
            }
            else
            {
                finalScore = -1;
            }
        }
    }
    public class ReviewTypeVM
    {
        public int id { get; set; }
        public int reviewId { get; set; }
        public int displayOrder { get; set; }
        public string name { get; set; }
        public int maxValue { get; set; }
        public string abbr { get; set; }
        public ReviewTypeVM(Room2ReviewType model)
        {
            id = model.Id;
            reviewId = model.ReviewTypeId;
            displayOrder = model.DisplayOrder;
            name = model.ReviewType.Name;
            maxValue = model.MaxValue;
            abbr = model.ReviewType.Abbr;
        }
    }
}