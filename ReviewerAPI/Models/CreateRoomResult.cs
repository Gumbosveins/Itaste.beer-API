using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewerAPI.Models
{
    public class CreateRoomResult : Result
    {
        public string roomCode { get; set; }
        public CreateRoomResult(string code) : base()
        {
            roomCode = code;
        }
        public CreateRoomResult(ResultStatus status, string message) : base(status, message)
        {

        }
    }
}