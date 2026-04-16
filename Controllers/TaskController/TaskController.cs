using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Task;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskInterface taskInterface;

        public TasksController(ITaskInterface _taskInterface)
        {
            taskInterface = _taskInterface;
        }

        // ===============================
        // 🔹 Get all tasks
        // GET: /api/tasks
        // ===============================
        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await taskInterface.GetAllTasks();
            return Ok(tasks);
        }

        // ===============================
        // 🔹 Get all tasks for a backlog
        // GET: /api/backlog-items/{backlogId}/tasks
        // ===============================
        [HttpGet("backlog-items/{backlogId}/tasks")]
        public async Task<IActionResult> GetAllTasksForBacklog(Guid backlogId)
        {
            var tasks = await taskInterface.GetAllTasksForBacklog(backlogId);
            return Ok(tasks);
        }

        // ===============================
        // 🔹 Get task by ID
        // GET: /api/tasks/{taskId}
        // ===============================
        [HttpGet("tasks/{taskId}")]
        public async Task<IActionResult> GetTaskById(Guid taskId)
        {
            var task = await taskInterface.GetTaskById(taskId);
            if (task == null) return NotFound();
            return Ok(task);
        }

        // ===============================
        // 🔹 Create task under backlog
        // POST: /api/backlog-items/{backlogId}/tasks
        // ===============================
        [HttpPost("backlog-items/{backlogId}/tasks")]
        public async Task<IActionResult> AddTaskToBacklog(Guid backlogId, [FromBody] TaskRequest taskRequest)
        {
            var task = await taskInterface.AddTaskToBacklog(backlogId, taskRequest);

            return CreatedAtAction(
                nameof(GetTaskById),
                new { taskId = task.Id },
                task
            );
        }

        // ===============================
        // 🔹 Update task
        // PUT: /api/tasks/{taskId}
        // ===============================
        [HttpPut("tasks/{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskRequest taskRequest)
        {
            var updatedTask = await taskInterface.UpdateTask(taskId, taskRequest);
            if (updatedTask == null) return NotFound();
            return Ok(updatedTask);
        }

        // ===============================
        // 🔹 Update task status
        // PATCH: /api/tasks/{taskId}/status
        // ===============================
        [HttpPatch("tasks/{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] string newStatus)
        {
            var updatedTask = await taskInterface.UpdateTaskStatus(taskId, newStatus);
            if (updatedTask == null) return NotFound();
            return Ok(updatedTask);
        }

        // ===============================
        // 🔹 Assign task to user
        // PATCH: /api/tasks/{taskId}/assign/{userId}
        // ===============================
        [HttpPatch("tasks/{taskId}/assign/{userId}")]
        public async Task<IActionResult> AssignTaskToUser(Guid taskId, Guid userId)
        {
            var updatedTask = await taskInterface.AssignTaskToUser(taskId, userId);
            if (updatedTask == null) return NotFound();
            return Ok(updatedTask);
        }

        // ===============================
        // 🔹 Delete task
        // DELETE: /api/tasks/{taskId}
        // ===============================
        [HttpDelete("tasks/{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
             await taskInterface.DeleteTask(taskId);
            return NoContent();
        }
    }
}