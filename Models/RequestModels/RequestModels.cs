using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ItbApi.Models.RequestModels
{
    public class CreateRoomRequest
    {
        public int pin { get; set; }
        public string roomName { get; set; }
        public bool blind { get; set; }
        public string revealCode { get; set; }
    }

    public class JoinRoomResponse : Result
    {
        public Guid userId { get; set; }
        public JoinRoomResponse(ResultStatus r, string message) : base (r, message)
        {

        }
        public JoinRoomResponse(Guid userId) : base(ResultStatus.SUCCESS, "Room Joined")
        {
            this.userId = userId;
        }
    }

    public class JoinRoomRequest
    {
        public string roomCode { get; set; }
        public int pin { get; set; }
        public string username { get; set; }
    }

    public class DeleteUserRequest
    {
        public Guid userId { get; set; }
    }


    public class AddReviewRequest
    {
        public int beverageId { get; set; }
        public Guid userId { get; set; }
        public string roomCode { get; set; }
        public List<AddReviewPart> parts { get; set; }
        public string comment { get; set; }


    }

    public class AddReviewPart
    {
        public int reviewTypeId { get; set; }
        public decimal score { get; set; }
    }

    public class FinishRoomSettings
    {
        public class Root : CreateRoomRequest
        {
            public string ipAddress { get; set; }
            public string roomCode { get; set; }
            public List<Beer> beers { get; set; }
            public List<ReviewCategory> categories { get; set; }
        }

        public class Beer
        {
            public int id { get; set; }
            public int displayOrder { get; set; }

        }

        public class ReviewCategory
        {
            public int id { get; set; }
            public int maxValue { get; set; }
            public int displayOrder { get; set; }
            public string name { get; set; }
            public string abbr { get; set; }
            public bool isNew { get; set; }
        }
    }

    public class AddNewBeverage
    {
        
        public string name { get; set; }
        public string manufacturer { get; set; }
        public decimal alcaholPercent { get; set; }
        public int majorGroupId { get; set; }
        public int type { get; set; }
        public string ipAddress { get; set; }
        public string imageUrl { get; set; }
        
    }

    public class AddNewReviewType
    {
        public string abbr { get; set; }
        public string name { get; set; }
        public string ipAddress { get; set; }

    }
    
}

