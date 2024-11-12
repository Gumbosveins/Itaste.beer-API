using ItbApi.DataAccessLayer;
using ItbApi.Models;
using ItbApi.Models.RequestModels;
using ItbApi.Models.ViewModels;

namespace ItbApi.TaterBusiness
{
    public interface ITasterBusiness
    {
        ITasterDal TasterDal { get; }

        Task<Result> AddBeer(int id);
        Task<string> AddBeers(string query);
        Task<CreateRoomResult> CreateRoom(CreateRoomRequest request);
        Task<Result> JoinRoom(JoinRoomRequest request);
        Task<List<BeerVM>> SearchUnTapped(string query, bool forceUntapped);
    }
}