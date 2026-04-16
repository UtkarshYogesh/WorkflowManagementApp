using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Backlog;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class BacklogItemsController : ControllerBase
    {
        private readonly IBacklogInterface backlogInterface;

        public BacklogItemsController(IBacklogInterface _backlogInterface)
        {
            backlogInterface = _backlogInterface;
        }

        // ===============================
        // 🔹 Get all backlog items
        // GET: /api/backlog-items
        // ===============================
        [HttpGet("backlog-items")]
        public async Task<IActionResult> GetAllBacklogs()
        {
            var backlogs = await backlogInterface.GetAllBacklogs();
            return Ok(backlogs);
        }

        // ===============================
        // 🔹 Get backlog items by feature
        // GET: /api/features/{featureId}/backlog-items
        // ===============================
        [HttpGet("features/{featureId}/backlog-items")]
        public async Task<IActionResult> GetBacklogsByFeatureId(Guid featureId)
        {
            var backlogs = await backlogInterface.GetBacklogsByFeatureId(featureId);
            return Ok(backlogs);
        }

        // ===============================
        // 🔹 Get backlog item by ID
        // GET: /api/backlog-items/{backlogId}
        // ===============================
        [HttpGet("backlog-items/{backlogId}")]
        public async Task<IActionResult> GetBacklogById(Guid backlogId)
        {
            var backlog = await backlogInterface.GetBacklogById(backlogId);
            if (backlog == null) return NotFound();
            return Ok(backlog);
        }

        // ===============================
        // 🔹 Create backlog under feature
        // POST: /api/features/{featureId}/backlog-items
        // ===============================
        [HttpPost("features/{featureId}/backlog-items")]
        public async Task<IActionResult> AddBacklogToFeature(Guid featureId, [FromBody] BacklogRequest backlogRequest)
        {
            var backlog = await backlogInterface.AddBacklogToFeature(featureId, backlogRequest);

            return CreatedAtAction(
                nameof(GetBacklogById),
                new { backlogId = backlog.Id },
                backlog
            );
        }

        // ===============================
        // 🔹 Update backlog item
        // PUT: /api/backlog-items/{backlogId}
        // ===============================
        [HttpPut("backlog-items/{backlogId}")]
        public async Task<IActionResult> UpdateBacklog(Guid backlogId, [FromBody] BacklogRequest backlogRequest)
        {
            var updatedBacklog = await backlogInterface.UpdateBacklog(backlogId, backlogRequest);
            if (updatedBacklog == null) return NotFound();
            return Ok(updatedBacklog);
        }

        // ===============================
        // 🔹 Update backlog status
        // PATCH: /api/backlog-items/{backlogId}/status
        // ===============================
        [HttpPatch("backlog-items/{backlogId}/status")]
        public async Task<IActionResult> UpdateBacklogStatus(Guid backlogId, [FromBody] string newStatus)
        {
            var updatedBacklog = await backlogInterface.UpdateBacklogStatus(backlogId, newStatus);
            if (updatedBacklog == null) return NotFound();
            return Ok(updatedBacklog);
        }

        // ===============================
        // 🔹 Assign backlog to user
        // PATCH: /api/backlog-items/{backlogId}/assign/{userId}
        // ===============================
        [HttpPatch("backlog-items/{backlogId}/assign/{userId}")]
        public async Task<IActionResult> AssignBacklogToUser(Guid backlogId, Guid userId)
        {
            var updatedBacklog = await backlogInterface.AssignBacklogToUser(backlogId, userId);
            if (updatedBacklog == null) return NotFound();
            return Ok(updatedBacklog);
        }

        // ===============================
        // 🔹 Delete backlog item
        // DELETE: /api/backlog-items/{backlogId}
        // ===============================
        [HttpDelete("backlog-items/{backlogId}")]
        public async Task<IActionResult> DeleteBacklog(Guid backlogId)
        {
            await backlogInterface.DeleteBacklog(backlogId);
            return NoContent();
        }
    }
}