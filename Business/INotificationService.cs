using ItbApi.Models;

namespace ReviewerAPI.BeerHubs
{
    public interface INotificationService
    {
        Task<string> JoinRoom(string roomCode, string connectionId);
        Task<string> JoinRoomAsUser(string roomCode, string connectionId);
        Task NewUserJoined(string roomCode, string username, Guid userId);
        Task OpenBeerForUser(string roomCode, int beerId);
        Task PushFinalScore(string roomCode, int beerId, decimal finalScore);
        Task PushReview(string roomCode, BeverageReviewVM model);
    }
}