using ItbApi.DataAccessLayer;
using ItbApi.Helpers;
using ItbApi.Models;
using ItbApi.Models.RequestModels;
using ItbApi.Models.ViewModels;
using ItbApi.TaterBusiness;
using Microsoft.AspNetCore.Mvc;
using ReviewerAPI.BeerHubs;
using System.Net;

namespace ItbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasterController(
        ITasterBusiness business,
        ITasterDal dal,
        IUnTapped unTapped,
        IHttpContextAccessor _httpContextAccessor,
        INotificationService notificationService) : ControllerBase
    {
        public ITasterBusiness Business { get; } = business;
        public ITasterDal Dal { get; } = dal;
        public IUnTapped UnTapped { get; } = unTapped;
        public IHttpContextAccessor HttpContextAccessor { get; } = _httpContextAccessor;
        public INotificationService NotificationService { get; } = notificationService;

        [HttpPost, Route("JoinRoom")]
        public async Task<JoinRoomResponse> JoinRoom(JoinRoomRequest request)
        {
            return await Dal.JoinRoom(request);
        }

        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            Result result = await Dal.DeleteUser(request);
            return ApiTools.CreateResponse(result);
        }


        [HttpPost, Route("CreateRoom")]
        public async Task<IActionResult> CreateRoom(CreateRoomRequest request)
        {
            Result result = await Business.CreateRoom(request);
            return ApiTools.CreateResponse(result);
        }

        [HttpPost, Route("FinishRoomCreation")]
        public async Task<IActionResult> FinishRoomCreation(FinishRoomSettings.Root request)
        {
            request.ipAddress = GetUserIP();
            Result result = await Dal.FinishRoomCreation(request);
            return ApiTools.CreateResponse(result);
        }

        [HttpPost, Route("AddBeverage")]
        public async Task<IActionResult> AddBeverage(AddNewBeverage request)
        {
            request.ipAddress = GetUserIP();
            Result result = await Dal.AddBeverage(request);
            return ApiTools.CreateResponse(result);
        }

        [HttpPost, Route("AddReviewType")]
        public async Task<IActionResult> AddReviewType(AddNewReviewType request)
        {
            request.ipAddress = GetUserIP();
            Result result = await Dal.AddReviewType(request);
            return ApiTools.CreateResponse(result);
        }

        [HttpPost, Route("AddReview")]
        public async Task<IActionResult> AddReview(AddReviewRequest reqiest)
        {
            Result result = await Dal.AddReview(reqiest);
            return ApiTools.CreateResponse(result);
        }

        [HttpGet, Route("GetRoom")]
        public async Task<UserRoomModel> GetRoom(string roomCode, int pin, Guid userId)
        {
            return await Dal.GetRoom(roomCode, pin, userId);
        }

        [HttpGet, Route("GetDashboard")]
        public async Task<Dashboard> GetDashboard(string roomCode, int pin)
        {
            Dashboard model = await Dal.GetDashboard(roomCode, pin);
            return model;
        }

        [HttpGet, Route("GetBreveries")]
        public async Task<List<BreweryVM>> GetBreveries()
        {
            var br = await Dal.GetAllBreweries();
            var breweries = br.Select(a => new BreweryVM(a)).OrderBy(a => a.name).ToList();
            breweries.Add(new BreweryVM()
            {
                id = 0,
                name = "Not listed"
            });

            return breweries;
        }

        [HttpGet, Route("GetBreverysBeers")]
        public async Task<List<BeerVM>> GetBreverysBeers(int id)
        {
            return await Dal.GetBreverysBeers(id);
        }

        [HttpGet, Route("Search")]
        public async Task<UnTappedBeerSearch.Root> Search(string query)
        {
            return await UnTapped.SearchUnTapped(query);
        
        }
            
        [HttpGet, Route("SearchForBeers")]
        public async Task<List<BeerVM>> SearchForBeers(string query, bool force)
        {
            return await Business.SearchUnTapped(query ,force);
        }

        [HttpGet, Route("GetReviewTypes")]
        public async Task<List<ReviewTypeForCreateVM>> GetReviewTypes()
        {
            List<ReviewTypeForCreateVM> types = new List<ReviewTypeForCreateVM>();
            types.Add(new ReviewTypeForCreateVM()
            {
                id = 0,
                isNew = false,
                name = "Add my own"
            });
            types.AddRange(await dal.GetReviewTypes());
            return types;

        }

        [HttpGet, Route("AddBeers")]
        public async Task<string> AddBeers(string query)
        {
            return await Business.AddBeers(query);
        }

        [HttpGet, Route("UnlockBeer")]
        public async Task<IActionResult> UnlockBeer(string roomCode, int beerId)
        {
            Result result = await Dal.UnlockBeer(roomCode, beerId);
            return ApiTools.CreateResponse(result);
        }

        [HttpGet, Route("FinishReview")]

        public Task<ReviewResults> FinishReview(string roomCode, int beerId)
        {
            return Dal.FinishReview(roomCode, beerId);
        }


        [HttpGet, Route("TestSignalR")]
        public async Task TestSignalR(string code, string name)
        {
            await NotificationService.NewUserJoined(code, name, Guid.NewGuid());
        }

        private string GetUserIP()
        {
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext == null) return string.Empty;

            string visitorIP = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                               ?? httpContext.Connection.RemoteIpAddress?.ToString();

            return visitorIP ?? string.Empty;
        }
    }
}
