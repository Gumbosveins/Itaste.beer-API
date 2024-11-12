using ItbApi.DataAccessLayer;
using ItbApi.Helpers;
using ItbApi.Models;
using ItbApi.Models.RequestModels;
using ItbApi.Models.ViewModels;

namespace ItbApi.TaterBusiness
{
    public class TasterBusiness(ITasterDal tasterDal, IUnTapped unTapped) : ITasterBusiness
    {
        private static Random random = new Random();

        public ITasterDal TasterDal { get; } = tasterDal;
        public IUnTapped UnTapped { get; } = unTapped;

        public async Task<CreateRoomResult> CreateRoom(CreateRoomRequest request)
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
                roomCodeTaken = await TasterDal.IsRoomCodeTaken(roomCode);
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

            return await this.TasterDal.CreateRoom(roomToAdd);
        }

        public async Task<Result> JoinRoom(JoinRoomRequest request)
        {
            Room roomToJoin = await this.TasterDal.GetRoomByCodeAndPin(request.roomCode, request.pin);
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

            return await this.TasterDal.CreateUser(userToAdd);
        }

        public static string GetRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        }



        public async Task<string> AddBeers(string query)
        {
            UnTappedBeerSearch.Root result = await UnTapped.SearchUnTapped(query);
            foreach (var res in result.response.beers.items)
            {
                UnTappedBeerInfo.Root info = UnTapped.GetBeerInfo(res.beer.bid).Result;
                await this.TasterDal.AddUnTappedBeer(info);
            }

            return "Vei";
        }

        public async Task<Result> AddBeer(int id)
        {
            UnTappedBeerInfo.Root info = await UnTapped.GetBeerInfo(id);
            await this.TasterDal.AddUnTappedBeer(info);
            return new Result();
        }

        public async Task<List<BeerVM>> SearchUnTapped(string query, bool forceUntapped)
        {
            List<BeerVM> beersToReturn = new List<BeerVM>();
            List<Beverage> beers = new List<Beverage>();
            if (!forceUntapped)
            {
                beers = await this.TasterDal.SearchForBeers(query);
                beers.ForEach(a => beersToReturn.Add(new BeerVM(a)));
            }

            if (!beers.Any() || forceUntapped)
            {
                UnTappedBeerSearch.Root result = await UnTapped.SearchUnTapped(query);
                beersToReturn = result.response.beers.items.Select(a => new BeerVM(a)).ToList();
            }
            return beersToReturn;
        }
    }
}