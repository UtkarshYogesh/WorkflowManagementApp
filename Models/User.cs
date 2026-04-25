namespace TaskManagement.Api.Models
{
    public class User
    {
        public Guid UserId { get; set; } 

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Member"; // Admin | Member

        public ICollection<Feature> AssignedFeatures { get; set; } = new List<Feature>();
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();

        public ICollection<BacklogItem> AssignBacklogItems { get; set; } = new List<BacklogItem>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public DateTime CreatedAt { get;  set; }
    }
}
