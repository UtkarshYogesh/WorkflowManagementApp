namespace TaskManagement.Api.DTOs.Task
{
    public class TaskResponse
    {
        public Guid Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Todo";
        public DateTime CreatedAt { get; set; } 
        public Guid BacklogItemId { get; set; }       // FK — now Guid
        public Guid? AssignedToUserId { get; set; }   // FK — now Guid, still nullable
    }
}
