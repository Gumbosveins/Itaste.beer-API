using ItbApi.Models;
using ItbApi.Models.RequestModels;
using ItbApi.Models.ViewModels;

namespace ItbApi.DataAccessLayer
{
    public interface ITasterDal
    {
        Task<Result> AddBeverage(AddNewBeverage request);
        Task<Result> AddReview(AddReviewRequest reqiest);
        Task<Result> AddReviewType(AddNewReviewType request);
        Task AddUnTappedBeer(UnTappedBeerInfo.Root info);
        Task<CreateRoomResult> CreateRoom(Room roomToAdd);
        Task<Result> CreateUser(User user);
        Task<Result> DeleteUser(DeleteUserRequest request);
        Task<ReviewResults> FinishReview(string roomCode, int beerId);
        Task<Result> FinishRoomCreation(FinishRoomSettings.Root request);
        Task<List<Brewery>> GetAllBreweries();
        Task<List<Beverage>> GetBeveragesByIds(IEnumerable<int> beverageIds);
        Task<List<BeverageType>> GetBeverageTypes();
        Task<List<BeerVM>> GetBreverysBeers(int breweryId);
        Task<Dashboard> GetDashboard(string roomCode, int pin);
        Task<List<ReviewTypeForCreateVM>> GetReviewTypes();
        Task<UserRoomModel> GetRoom(string roomCode, int pin, Guid userId);
        Task<Room> GetRoomByCodeAndPin(string code, int pin);
        Task<bool> IsRoomCodeTaken(string roomCode);
        Task<JoinRoomResponse> JoinRoom(JoinRoomRequest request);
        Task<List<Beverage>> SearchForBeers(string query);
        Task<Result> UnlockBeer(string roomCode, int beerId);
    }
}