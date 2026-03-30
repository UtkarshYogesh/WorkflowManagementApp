namespace TaskManagement.Api.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Todo";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid BacklogItemId { get; set; }       // FK — now Guid
        public BacklogItem BacklogItem { get; set; } = null!;

        public Guid? AssignedToUserId { get; set; }   // FK — now Guid, still nullable
        public User? AssignedToUser { get; set; }
    }
}
