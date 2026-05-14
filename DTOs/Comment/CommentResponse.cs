using TaskManagement.Api.Models.Enums;

namespace TaskManagement.Api.DTOs.Comment
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public string Body { get; set; } = string.Empty;
        public EntityEnums EntityType { get; set; } 
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }

        public Guid CreatedByUserId { get; set; }
        public ICollection<Guid> MentionedUserIds { get; set; } = new List<Guid>();
    }
}
