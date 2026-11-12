namespace ItbApi.Models.RequestModels
{
    public class EditRoomRequest
    {
        public string roomCode { get; set; } = "";
        public int pin { get; set; }
    }

    public class AddBeveragesRequest : EditRoomRequest
    {
        public List<int> beverageIds { get; set; } = new();
    }

    public class RemoveBeverageRequest : EditRoomRequest
    {
        public int beverageId { get; set; }
    }

    public class ReorderBeveragesRequest : EditRoomRequest
    {
        public List<int> beverageIds { get; set; } = new();
    }

    public class UpdateCategoriesRequest : EditRoomRequest
    {
        public List<FinishRoomSettings.ReviewCategory> categories { get; set; } = new();
    }

    public class RemoveUserRequest : EditRoomRequest
    {
        public Guid userId { get; set; }
    }
}
