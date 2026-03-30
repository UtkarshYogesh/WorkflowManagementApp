namespace TaskManagement.Api.Models
{
    public class BacklogItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid FeatureId { get; set; }           // FK — now Guid
        public Feature Feature { get; set; } = null!;

        public Guid? AssignedToUserId { get; set; }   // FK — now Guid, still nullable
        public User? AssignedToUser { get; set; }
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
