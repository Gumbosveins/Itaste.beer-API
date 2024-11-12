using ItbApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace ItbApi.Helpers
{
    public static class ApiTools
    {
        public static IActionResult CreateResponse(Result result)
        {
            switch (result.status)
            {
                case ResultStatus.SUCCESS:
                    return new OkObjectResult(result);

                case ResultStatus.BAD_DATA:
                    return new BadRequestObjectResult(result.message);

                case ResultStatus.NOT_FOUND:
                    return new NotFoundObjectResult(result.message);

                case ResultStatus.IMPOSSIBLE:
                    return new ObjectResult(result.message) { StatusCode = (int)HttpStatusCode.Forbidden };

                default:
                    return new ObjectResult(result.message) { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }
    }
}
