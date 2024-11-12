using Microsoft.AspNetCore.SignalR;

namespace ReviewerAPI.BeerHubs
{
    public class BeerHub : Hub
    {
        private readonly INotificationService _notificationService;

        public BeerHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Client connected: " + Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task<string> JoinRoom(string roomCode)
        {
            return await _notificationService.JoinRoom(roomCode, Context.ConnectionId);
        }

        public async Task<string> JoinRoomAsUser(string roomCode)
        {
            return await _notificationService.JoinRoomAsUser(roomCode, Context.ConnectionId);
        }
    }
}