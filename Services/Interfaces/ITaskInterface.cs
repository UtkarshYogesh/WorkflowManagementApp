using TaskManagement.Api.DTOs.Task;

namespace TaskManagement.Api.Services.Interfaces
{
    public interface ITaskInterface
    {
        Task<List<TaskResponse>> GetAllTasksForBacklog(Guid backlogId);
        Task<TaskResponse> GetTaskById(Guid taskId);
        Task<TaskResponse> AddTaskToBacklog(Guid backlogId, TaskRequest taskRequest);
        Task<TaskResponse> UpdateTaskStatus(Guid taskId, string newStatus);

        Task DeleteTask(Guid taskId);

        Task<TaskResponse> AssignTaskToUser(Guid taskId, Guid userId);
        Task<TaskResponse> UpdateTask(Guid guid, TaskRequest taskRequest);

    }
}
