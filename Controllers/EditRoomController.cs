using ItbApi.DataAccessLayer;
using ItbApi.Helpers;
using ItbApi.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace ItbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditRoomController(IRoomEditDal dal, IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        [HttpPost("AddBeverages")]
        public async Task<IActionResult> AddBeverages(AddBeveragesRequest request) =>
            ApiTools.CreateResponse(await dal.AddBeverages(request));

        [HttpPost("RemoveBeverage")]
        public async Task<IActionResult> RemoveBeverage(RemoveBeverageRequest request) =>
            ApiTools.CreateResponse(await dal.RemoveBeverage(request));

        [HttpPost("ReorderBeverages")]
        public async Task<IActionResult> ReorderBeverages(ReorderBeveragesRequest request) =>
            ApiTools.CreateResponse(await dal.ReorderBeverages(request));

        [HttpPost("UpdateCategories")]
        public async Task<IActionResult> UpdateCategories(UpdateCategoriesRequest request) =>
            ApiTools.CreateResponse(await dal.UpdateCategories(request, GetUserIp()));

        [HttpPost("RemoveUser")]
        public async Task<IActionResult> RemoveUser(RemoveUserRequest request) =>
            ApiTools.CreateResponse(await dal.RemoveUser(request));

        private string GetUserIp()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return string.Empty;
            }
            return httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                ?? string.Empty;
        }
    }
}
