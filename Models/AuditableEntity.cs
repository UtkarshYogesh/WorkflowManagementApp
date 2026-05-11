namespace TaskManagement.Api.Models
{
    public abstract class AuditableEntity
    {
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? UpdatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? DeletedByUserId { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
