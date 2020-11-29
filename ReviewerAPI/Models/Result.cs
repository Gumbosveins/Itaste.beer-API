using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewerAPI.Models
{
    public class Result
    {
        public ResultStatus status { get; set; }
        public string message { get; set; }
        public Result(ResultStatus status, string message)
        {
            this.status = status;
            this.message = message;
        }

        public Result()
        {
            this.status = ResultStatus.SUCCESS;
            this.message = "Ok";
        }
    }

    public enum ResultStatus
    {
        SUCCESS,    // Everything went according to plan. Should result in 200
        NOT_FOUND,  // Resource not found, does not exist in db. Should result in 404
        ERROR,      // Unexpected error. Should result in 500
        BAD_DATA,   // The caller has malformed parameter data. Should result in 401
        IMPOSSIBLE  // Some business rule has been broken. Should result in 403
    }
}