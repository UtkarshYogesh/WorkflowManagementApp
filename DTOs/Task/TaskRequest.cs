namespace TaskManagement.Api.DTOs.Task
{
    public class TaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public Guid? AssignedToUserId { get; set; }
    }
}
