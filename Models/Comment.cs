using TaskManagement.Api.Models.Enums;

namespace TaskManagement.Api.Models
{
    public class Comment : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Body { get; set; } = string.Empty;

        public Guid EntityId { get; set; }
        public EntityEnums EntityType { get; set; }

        public ICollection<MentionComment> MentionComments { get; set; } = new List<MentionComment>();
    }
}
