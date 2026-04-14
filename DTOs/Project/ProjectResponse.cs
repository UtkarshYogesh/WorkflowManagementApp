namespace TaskManagement.Api.DTOs.Project
{
    public class ProjectResponse
    {
        public Guid ProjectId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get;  set; }
    }
}
