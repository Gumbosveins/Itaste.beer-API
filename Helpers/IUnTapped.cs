using ItbApi.Models;

namespace ItbApi.Helpers
{
    public interface IUnTapped
    {
        Task<UnTappedBeerInfo.Root> GetBeerInfo(int beerId);
        Task<UnTappedBeerSearch.Root> SearchUnTapped(string query);
    }
}