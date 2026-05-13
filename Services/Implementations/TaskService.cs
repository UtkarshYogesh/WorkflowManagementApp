using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Task;
using TaskManagement.Api.Helpers;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class TaskService : ITaskInterface
    {
        public readonly AppDbContext _db;
        private readonly ICurrentUserService currentUser;

        public TaskService(AppDbContext appDbContext, ICurrentUserService currentUser)
        {
            _db = appDbContext;
            this.currentUser = currentUser;
        }

        public async Task<List<TaskResponse>> GetAllTasks()
        {
            return await _db.Tasks
                .Where(t => !t.IsDeleted && !t.BacklogItem.IsDeleted && !t.BacklogItem.Feature.IsDeleted)
                .Select(t => ToResponse(t))
                .ToListAsync();
        }

        public async Task<List<TaskResponse>> GetAllTasksForBacklog(Guid backlogId)
        {
            return await _db.Tasks
                .Where(t => t.BacklogItemId == backlogId && !t.IsDeleted && !t.BacklogItem.IsDeleted)
                .Select(t => ToResponse(t))
                .ToListAsync();
        }

        public async Task<TaskResponse> GetTaskById(Guid taskId)
        {
            var task = await _db.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted && !t.BacklogItem.IsDeleted);

            return task == null ? null : ToResponse(task);
        }

        public async Task<TaskResponse> AddTaskToBacklog(Guid backlogId, TaskRequest taskRequest)
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = taskRequest.Title,
                Description = taskRequest.Description,
                Status = StatusHelper.NormalizeTaskStatus(null),
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUser.UserId,
                BacklogItemId = backlogId,
                AssignedToUserId = taskRequest.AssignedToUserId
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return ToResponse(task);
        }

        public async Task<TaskResponse> UpdateTaskStatus(Guid taskId, string newStatus)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
            if (task == null) return null;

            task.Status = StatusHelper.NormalizeTaskStatus(newStatus);
            SetUpdated(task);
            await _db.SaveChangesAsync();

            return ToResponse(task);
        }

        public async Task<bool> DeleteTask(Guid taskId)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
            if (task == null || !CanDelete(task)) return false;

            task.IsDeleted = true;
            task.DeletedByUserId = currentUser.UserId;
            task.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<TaskResponse> AssignTaskToUser(Guid taskId, Guid userId)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
            if (task == null) return null;

            task.AssignedToUserId = userId;
            SetUpdated(task);
            await _db.SaveChangesAsync();

            return ToResponse(task);
        }

        public async Task<TaskResponse> UpdateTask(Guid guid, TaskRequest taskRequest)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == guid && !t.IsDeleted);
            if (task == null) return null;

            task.Title = taskRequest.Title;
            task.Description = taskRequest.Description;
            SetUpdated(task);
            await _db.SaveChangesAsync();

            return ToResponse(task);
        }

        private bool CanDelete(TaskItem task)
        {
            return currentUser.IsAdmin || task.CreatedByUserId == currentUser.UserId;
        }

        private void SetUpdated(TaskItem task)
        {
            task.UpdatedByUserId = currentUser.UserId;
            task.UpdatedAt = DateTime.UtcNow;
        }

        private static TaskResponse ToResponse(TaskItem task)
        {
            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                CreatedByUserId = task.CreatedByUserId,
                BacklogItemId = task.BacklogItemId,
                AssignedToUserId = task.AssignedToUserId
            };
        }
    }
}
