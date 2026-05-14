namespace TaskManagement.Api.Models
{
    public class MentionComment
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }

        public Comment Comment { get; set; } = null!;
        public Guid MentionedUserId { get; set; }
    }
}
