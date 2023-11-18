using ReviewerAPI.BeerHubs;
using ReviewerAPI.DataAccessLayer;
using ReviewerAPI.Helpers;
using ReviewerAPI.Models;
using ReviewerAPI.Models.Connection;
using ReviewerAPI.Models.RequestModels;
using ReviewerAPI.Models.ViewModels;
using ReviewerAPI.TaterBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ReviewerAPI.Controllers
{
    [RoutePrefix("api/Taster")]
    public class TasterController : ApiController
    {
        private readonly TasterBusiness business = new TasterBusiness();
        private readonly TasterDal dal = new TasterDal();


        [HttpPost, Route("JoinRoom")]
        public JoinRoomResponse JoinRoom(JoinRoomRequest request)
        {
            return dal.JoinRoom(request);
        }

        [HttpPost, Route("DeleteUser")]
        public HttpResponseMessage DeleteUser(DeleteUserRequest request)
        {
            Result result = dal.DeleteUser(request);
            return ApiTools.CreateResponse(result, Request);
        }

        
        [HttpPost, Route("CreateRoom")]
        public HttpResponseMessage CreateRoom(CreateRoomRequest request)
        {
            Result result = business.CreateRoom(request);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost, Route("FinishRoomCreation")]
        public async Task<HttpResponseMessage> FinishRoomCreation(FinishRoomSettings.Root request)
        {
            request.ipAddress = GetUser_IP();
            Result result = await dal.FinishRoomCreation(request);
            return ApiTools.CreateResponse(result, Request);
        }

        [HttpPost, Route("AddBeverage")]
        public HttpResponseMessage AddBeverage(AddNewBeverage request)
        {
            request.ipAddress = GetUser_IP();
            Result result = dal.AddBeverage(request);
            return ApiTools.CreateResponse(result, Request);
        }

        [HttpPost, Route("AddReviewType")]
        public HttpResponseMessage AddReviewType(AddNewReviewType request)
        {
            request.ipAddress = GetUser_IP();
            Result result = dal.AddReviewType(request);
            return ApiTools.CreateResponse(result, Request);
        }

        [HttpPost, Route("AddReview")]
        public HttpResponseMessage AddReview(AddReviewRequest reqiest)
        {
            Result result = dal.AddReview(reqiest);
            return ApiTools.CreateResponse(result, Request);
        }

        [HttpGet, Route("GetRoom")]
        public UserRoomModel GetRoom(string roomCode, int pin, Guid userId)
        {
            bool userCache = false;
            string key = "room";
            if (Cache.Has(key) && userCache)
                return Cache.Get<UserRoomModel>(key);
            UserRoomModel model = dal.GetRoom(roomCode, pin, userId);
            Cache.Add(model, key, Cache.CacheDuration.Day);
            return model;
        }

        [HttpGet, Route("GetDashboard")]
        public Dashboard GetDashboard(string roomCode, int pin)
        {
            bool userCache = false;
            string key = "dashboard";
            if (Cache.Has(key) && userCache)
                return Cache.Get<Dashboard>(key);
            Dashboard model = dal.GetDashboard(roomCode, pin);
            Cache.Add(model, key, Cache.CacheDuration.Day);
            return model;
        }

        [HttpGet, Route("GetBreveries")]
        public List<BreweryVM> GetBreveries()
        {
            List<BreweryVM> breweries = dal.GetAllBreweries().Select(a => new BreweryVM(a)).OrderBy(a => a.name).ToList();
            breweries.Add(new BreweryVM()
            {
                id = 0,
                name = "Not listed"
            });

            return breweries;
        }

        [HttpGet, Route("GetBreverysBeers")]
        public List<BeerVM> GetBreverysBeers(int id)
        {
            return dal.GetBreverysBeers(id);
        }

        [HttpGet, Route("Search")]
        public async Task<UnTappedBeerSearch.Root> Search(string query)
        {
            UnTapped untappedAsync = new UnTapped();
            return await untappedAsync.SearchUnTapped(query);
        
        }
            
        [HttpGet, Route("SearchForBeers")]
        public async Task<List<BeerVM>> SearchForBeers(string query, bool force)
        {
            TasterBusiness business = new TasterBusiness();
            return await business.SearchUnTapped(query ,force);
        }

        [HttpGet, Route("GetReviewTypes")]
        public List<ReviewTypeForCreateVM> GetReviewTypes()
        {
            TasterDal dal = new TasterDal();
            List<ReviewTypeForCreateVM> types = new List<ReviewTypeForCreateVM>();
            types.Add(new ReviewTypeForCreateVM()
            {
                id = 0,
                isNew = false,
                name = "Add my own"
            });
            types.AddRange(dal.GetReviewTypes());
            return types;

        }

        [HttpGet, Route("AddBeers")]
        public async Task<string> AddBeers(string query)
        {
            TasterBusiness business = new TasterBusiness();
            return await business.AddBeers(query);
        }

        [HttpGet, Route("UnlockBeer")]
        public HttpResponseMessage UnlockBeer(string roomCode, int beerId)
        {
            TasterDal dal = new TasterDal();
            Result result = dal.UnlockBeer(roomCode, beerId);
            return ApiTools.CreateResponse(result, Request);
        }

        [HttpGet, Route("FinishReview")]

        public ReviewResults FinishReview(string roomCode, int beerId)
        {
            TasterDal dal = new TasterDal();
            return dal.FinishReview(roomCode, beerId);
        }


        [HttpGet, Route("TestSignalR")]
        public void TestSignalR(string code, string name)
        {
            beerhub hub = new beerhub();
            hub.NewUserJoined(code, name, Guid.NewGuid());

        }

        private string GetUser_IP()
        {
            string visitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                visitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                visitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }
            return visitorsIPAddr;
        }
    }
}
