namespace TaskManagement.Api.Models
{
    public class Project : AuditableEntity
    {
        public Guid ProjectId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<Feature> Features { get; set; } = new List<Feature>();
    }
}
