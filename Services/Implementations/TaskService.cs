using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Task;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class TaskService: ITaskInterface
    {
        public readonly AppDbContext _db;

        public TaskService(AppDbContext appDbContext) { _db = appDbContext; }

        public async Task<List<TaskResponse>> GetAllTasks()
        {
            var tasks = await _db.Tasks
                .Select(t => new TaskResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    BacklogItemId = t.BacklogItemId,
                    AssignedToUserId = t.AssignedToUserId
                })
                .ToListAsync();

            return tasks;
        }

        public async Task<List<TaskResponse>> GetAllTasksForBacklog(Guid backlogId)
        {
            var tasks = await _db.Tasks.Where(t => t.BacklogItemId == backlogId)
                .Select(t => new TaskResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    BacklogItemId = t.BacklogItemId,
                    AssignedToUserId = t.AssignedToUserId
                })
                .ToListAsync();

            return tasks;

        }

        public async Task<TaskResponse> GetTaskById(Guid taskId)
        {
            var task = await _db.Tasks.Where(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task == null) return null;
            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                BacklogItemId = task.BacklogItemId,
                AssignedToUserId = task.AssignedToUserId
            };
        }

        public async Task<TaskResponse> AddTaskToBacklog(Guid backlogId, TaskRequest taskRequest)
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = taskRequest.Title,
                Description = taskRequest.Description,
                Status = "Todo",
                CreatedAt = DateTime.UtcNow,
                BacklogItemId = backlogId,
                AssignedToUserId = taskRequest.AssignedToUserId
            };
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                BacklogItemId = task.BacklogItemId,
                AssignedToUserId = task.AssignedToUserId
            };
        }

        public async Task<TaskResponse> UpdateTaskStatus(Guid taskId, string newStatus)
        {
            var task = await _db.Tasks.Where(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task == null) return null;
            task.Status = newStatus;
            await _db.SaveChangesAsync();
            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                BacklogItemId = task.BacklogItemId,
                AssignedToUserId = task.AssignedToUserId
            };
        }

        public async Task DeleteTask(Guid taskId)
        {
            var task = _db.Tasks.Where(t => t.Id == taskId).FirstOrDefault();
            if (task != null)
            {
                _db.Tasks.Remove(task);
                _db.SaveChanges();
            }

        }

        public async Task<TaskResponse> AssignTaskToUser(Guid taskId, Guid userId)
        {
            var task = await _db.Tasks.Where(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task == null) return null;
            task.AssignedToUserId = userId;
            await _db.SaveChangesAsync();
            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                BacklogItemId = task.BacklogItemId,
                AssignedToUserId = task.AssignedToUserId
            };
        }

        public async Task<TaskResponse> UpdateTask(Guid guid, TaskRequest taskRequest)
        {
            var task = await _db.Tasks.Where(t => t.Id == guid).FirstOrDefaultAsync();
            if (task == null) return null;
            task.Title = taskRequest.Title;
            task.Description = taskRequest.Description;
            await _db.SaveChangesAsync();

            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                BacklogItemId = task.BacklogItemId,
                AssignedToUserId = task.AssignedToUserId
            };
        }
    }
}
