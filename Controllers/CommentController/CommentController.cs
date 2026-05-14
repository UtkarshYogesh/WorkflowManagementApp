using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Comment;
using TaskManagement.Api.Models.Enums;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentInterface commentInterface;

        public CommentsController(ICommentInterface commentInterface)
        {
            this.commentInterface = commentInterface;
        }

        [HttpGet("comments/{commentId}")]
        public async Task<IActionResult> GetCommentById(Guid commentId)
        {
            var comment = await commentInterface.GetCommentById(commentId);
            if (comment == null) return NotFound();
            return Ok(comment);
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetCommentsForEntity([FromQuery] Guid entityId, [FromQuery] EntityEnums entityType)
        {
            var comments = await commentInterface.GetCommentsForEntity(entityId, entityType);
            return Ok(comments);
        }

        [HttpPost("comments")]
        public async Task<IActionResult> CreateComment([FromBody] CommentRequest commentRequest)
        {
            try
            {
                var comment = await commentInterface.CreateComment(commentRequest);
                if (comment == null) return NotFound();

                return CreatedAtAction(
                    nameof(GetCommentById),
                    new { commentId = comment.Id },
                    comment
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] CommentRequest commentRequest)
        {
            try
            {
                var comment = await commentInterface.UpdateComment(commentId, commentRequest);
                if (comment == null) return NotFound();
                return Ok(comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var deleted = await commentInterface.DeleteComment(commentId);
            if (!deleted) return Forbid();
            return NoContent();
        }
    }
}
