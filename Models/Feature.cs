namespace TaskManagement.Api.Models
{
    public class Feature
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Planned";
        public DateTime CreatedAt { get; set; } 

        public Guid ProjectId { get; set; }           // FK — now Guid
        public Project Project { get; set; } = null!;

        public Guid? AssignedToUserId { get; set; }   // FK — now Guid, still nullable
        public User? AssignedToUser { get; set; }

        public ICollection<BacklogItem> BacklogItems { get; set; } = new List<BacklogItem>();
    }
}
