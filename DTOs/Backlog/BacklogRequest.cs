namespace TaskManagement.Api.DTOs.Backlog
{
    public class BacklogRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "P3";
        public string Type { get; set; } = "Story";
        public Guid? AssignedToUserId { get; set; }   // FK — now Guid, still nullable
    }
}
