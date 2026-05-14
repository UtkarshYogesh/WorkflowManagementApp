using TaskManagement.Api.DTOs.Comment;
using TaskManagement.Api.Models.Enums;

namespace TaskManagement.Api.Services.Interfaces
{
    public interface ICommentInterface
    {
        Task<CommentResponse?> CreateComment(CommentRequest commentRequest);
        Task<CommentResponse?> GetCommentById(Guid commentId);
        Task<List<CommentResponse>> GetCommentsForEntity(Guid entityId, EntityEnums entityType);
        Task<CommentResponse?> UpdateComment(Guid commentId, CommentRequest commentRequest);
        Task<bool> DeleteComment(Guid commentId);
    }
}
