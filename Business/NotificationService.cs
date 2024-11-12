using ItbApi.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace ReviewerAPI.BeerHubs
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<BeerHub> _hubContext;

        public NotificationService(IHubContext<BeerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<string> JoinRoom(string roomCode, string connectionId)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, "Room:" + roomCode.ToLower());
            return "Socket connected to Room";
        }

        public async Task<string> JoinRoomAsUser(string roomCode, string connectionId)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, "UserRoom:" + roomCode.ToLower());
            return "Socket connected to UserRoom";
        }

        public async Task PushReview(string roomCode, BeverageReviewVM model)
        {
            await _hubContext.Clients.Group("Room:" + roomCode.ToLower()).SendAsync("NewReview", model);
        }

        public async Task NewUserJoined(string roomCode, string username, Guid userId)
        {
            var data = new { username, userId };
            await _hubContext.Clients.Group("Room:" + roomCode.ToLower()).SendAsync("NewUserJoined", data);
        }

        public async Task OpenBeerForUser(string roomCode, int beerId)
        {
            await _hubContext.Clients.Group("UserRoom:" + roomCode.ToLower()).SendAsync("OpenBeer", beerId);
        }

        public async Task PushFinalScore(string roomCode, int beerId, decimal finalScore)
        {
            var result = new { beerId, finalScore };
            await _hubContext.Clients.Group("UserRoom:" + roomCode.ToLower()).SendAsync("PushFinalScore", result);
        }
    }
}