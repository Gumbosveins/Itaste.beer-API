using ItbApi.Models;
using ItbApi.Models.RequestModels;

namespace ItbApi.DataAccessLayer
{
    public interface IRoomEditDal
    {
        Task<Result> AddBeverages(AddBeveragesRequest request);
        Task<Result> RemoveBeverage(RemoveBeverageRequest request);
        Task<Result> ReorderBeverages(ReorderBeveragesRequest request);
        Task<Result> UpdateCategories(UpdateCategoriesRequest request, string ipAddress);
        Task<Result> RemoveUser(RemoveUserRequest request);
    }
}
