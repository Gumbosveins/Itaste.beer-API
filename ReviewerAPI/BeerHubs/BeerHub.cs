using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using ReviewerAPI.Controllers;
using ReviewerAPI.Models;
using System.Threading;

namespace ReviewerAPI.BeerHubs
{
    [HubName("beerhub")]
    public class beerhub : Hub
    {
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<beerhub>();
        public void Hello()
        {
            hub.Clients.Group("Room:" + "LHYU").Hello();
        }

        public string JoinRoom(string roomCode)
        {
            string connectionId = Context.ConnectionId;
            hub.Groups.Add(connectionId, "Room:" + roomCode.ToLower());
            return "Socket connected";
        }

        public string JoinRoomAsUser(string roomCode)
        {
            string connectionId = Context.ConnectionId;
            hub.Groups.Add(connectionId, "UserRoom:" + roomCode.ToLower());
            return "Socket connected";
        }

        public void PushReview(string roomCode, BeverageReviewVM model)
        {
            hub.Clients.Group("Room:" + roomCode.ToLower()).NewReview(model);
        }

        internal void NewUserJoined(string roomCode, string username, Guid userId)
        {
            var data = new
            {
                username,
                userId
            };
            hub.Clients.Group("Room:" + roomCode.ToLower()).NewUserJoined(data);
        }

        public void OpenBeerForUser(string roomCode, int beerId)
        {
            hub.Clients.Group("UserRoom:" + roomCode.ToLower()).OpenBeer(beerId);
        }

        public void PushFinalScore(string roomCode, int beerId, decimal finalScore)
        {
            var result = new {
                             beerId,
                             finalScore
                         };
            hub.Clients.Group("UserRoom:" + roomCode.ToLower()).PushFinalScore(result);
        }
    }
}