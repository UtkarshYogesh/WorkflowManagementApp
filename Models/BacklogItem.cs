namespace TaskManagement.Api.Models
{
    public class BacklogItem : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
        public string Priority { get; set; } = "P3";
        public string Type { get; set; } = "Story";

        public Guid FeatureId { get; set; }           // FK — now Guid
        public Feature Feature { get; set; } = null!;

        public Guid? AssignedToUserId { get; set; }   // FK — now Guid, still nullable
        public User? AssignedToUser { get; set; }
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
