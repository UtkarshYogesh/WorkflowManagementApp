using TaskManagement.Api.Models.Enums;

namespace TaskManagement.Api.DTOs.Comment
{
    public class CommentRequest
    {
        public Guid EntityId { get; set; }
        public string Body { get; set; } = string.Empty;
        public EntityEnums EntityType { get; set; } 

        public ICollection<Guid> MentionUserIds { get; set; } = new List<Guid>();
    }
}
