using ReviewerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace ReviewerAPI.Helpers
{
    public class ApiTools
    {
        public static HttpResponseMessage CreateResponse(Result result, HttpRequestMessage request)
        {
            if (result.status == ResultStatus.SUCCESS)
            {
                return request.CreateResponse(HttpStatusCode.OK, result.message);

            }
            else if (result.status == ResultStatus.BAD_DATA)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, result.message);
            }
            else if (result.status == ResultStatus.NOT_FOUND)
            {
                return request.CreateErrorResponse(HttpStatusCode.NotFound, result.message);
            }
            else if (result.status == ResultStatus.IMPOSSIBLE)
            {
                return request.CreateErrorResponse(HttpStatusCode.Forbidden, result.message);
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.message);
            }

        }
    }
}