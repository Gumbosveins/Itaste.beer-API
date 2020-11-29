using ReviewerAPI.DataAccessLayer;
using ReviewerAPI.Helpers;
using ReviewerAPI.Models;
using ReviewerAPI.Models.Connection;
using ReviewerAPI.Models.RequestModels;
using ReviewerAPI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ReviewerAPI.TaterBusiness
{
    public class TasterBusiness
    {
        private static Random random = new Random();
        private TasterDal dal = new TasterDal();
        internal CreateRoomResult CreateRoom(CreateRoomRequest request)
        {

            if (String.IsNullOrEmpty(request.roomName))
                return new CreateRoomResult(ResultStatus.BAD_DATA, "Room name missing!");

            if (request.pin < 1000 || request.pin > 9999)
                return new CreateRoomResult(ResultStatus.BAD_DATA, "Room Pin needs to be four digits");

            List<string> types = new List<string>() { "Beer", "Wine", "Whiskey", "Cognac" };

            string roomCode = "";
            bool roomCodeTaken = true;
            while (roomCodeTaken)
            {
                roomCode = GetRoomCode();
                roomCodeTaken = dal.IsRoomCodeTaken(roomCode);
            }

            int roomType = 0;

            if (request.blind)
            {
                roomType = 1;
            }

            Room roomToAdd = new Room()
            {
                Code = roomCode,
                DateCreated = DateTime.Now,
                Name = request.roomName,
                Pin = request.pin,
                Owner = "",
                RoomType = roomType,
                BlindRevealCode = request.revealCode
            };

            return dal.CreateRoom(roomToAdd);
        }

        internal Result JoinRoom(JoinRoomRequest request)
        {
            Room roomToJoin = dal.GetRoomByCodeAndPin(request.roomCode, request.pin);
            if (roomToJoin == null)
            {
                return new Result(ResultStatus.BAD_DATA, "Room or pin is uncorrect");
            }

            User userToAdd = new User()
            {
                IsOwner = false,
                DateCreated = DateTime.Now,
                Name = request.username,
                RoomId = roomToJoin.Id
            };

            return dal.CreateUser(userToAdd);
        }

        public static string GetRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        }



        internal async Task<string> AddBeers(string query)
        {
            TasterDal dal = new TasterDal();
            UnTapped unTapped = new UnTapped();
            UnTappedBeerSearch.Root result = await unTapped.SearchUnTapped(query);
            foreach (var res in result.response.beers.items)
            {
                UnTappedBeerInfo.Root info = unTapped.GetBeerInfo(res.beer.bid).Result;
                dal.AddUnTappedBeer(info);
            }
            return "Vei";
        }

        internal async Task<Result> AddBeer(int id)
        {
            UnTapped unTapped = new UnTapped();
            UnTappedBeerInfo.Root info = await unTapped.GetBeerInfo(id);
            dal.AddUnTappedBeer(info);
            return new Result();
        }

        internal async Task<List<BeerVM>> SearchUnTapped(string query, bool forceUntapped)
        {
            List<BeerVM> beersToReturn = new List<BeerVM>();
            List<Beverage> beers = new List<Beverage>();
            if (!forceUntapped)
            {
                beers = dal.SearchForBeers(query);
                beers.ForEach(a => beersToReturn.Add(new BeerVM(a)));
            }

            if (!beers.Any() || forceUntapped)
            {
                UnTapped unTapped = new UnTapped();
                UnTappedBeerSearch.Root result = await unTapped.SearchUnTapped(query);
                beersToReturn = result.response.beers.items.Select(a => new BeerVM(a)).ToList();
            }
            return beersToReturn;
        }
    }
}