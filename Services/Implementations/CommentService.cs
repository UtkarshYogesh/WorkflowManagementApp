using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Comment;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.Enums;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class CommentService : ICommentInterface
    {
        private readonly AppDbContext _db;
        private readonly ICurrentUserService currentUser;
        private readonly ILogger<CommentService> _logger;

        public CommentService(AppDbContext appDbContext, ICurrentUserService currentUser, ILogger<CommentService> logger)
        {
            _db = appDbContext;
            this.currentUser = currentUser;
            _logger = logger;
        }

        public async Task<CommentResponse?> CreateComment(CommentRequest commentRequest)
        {
            var body = NormalizeBody(commentRequest.Body);
            var mentionedUserIds = NormalizeMentionUserIds(commentRequest.MentionUserIds);

            if (!await ExistEntityAsync(commentRequest.EntityType, commentRequest.EntityId))
            {
                _logger.LogWarning("Comment creation failed because {EntityType} {EntityId} was not found", commentRequest.EntityType, commentRequest.EntityId);
                return null;
            }

            await ValidateMentionedUsersAsync(mentionedUserIds);

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Body = body,
                CreatedAt = DateTime.UtcNow,
                EntityId = commentRequest.EntityId,
                EntityType = commentRequest.EntityType,
                CreatedByUserId = currentUser.UserId,
            };

            _db.Comments.Add(comment);

            var mentionComments = mentionedUserIds.Select(userId => new MentionComment
            {
                Id = Guid.NewGuid(),
                CommentId = comment.Id,
                MentionedUserId = userId
            }).ToList();

            await _db.MentionComments.AddRangeAsync(mentionComments);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Comment {CommentId} created on {EntityType} {EntityId} by user {UserId}", comment.Id, comment.EntityType, comment.EntityId, currentUser.UserId);
            return ToResponse(comment, mentionedUserIds);
        }

        public async Task<CommentResponse?> GetCommentById(Guid commentId)
        {
            var comment = await _db.Comments
                .AsNoTracking()
                .Include(c => c.MentionComments)
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

            return comment == null ? null : ToResponse(comment);
        }

        public async Task<List<CommentResponse>> GetCommentsForEntity(Guid entityId, EntityEnums entityType)
        {
            if (!await ExistEntityAsync(entityType, entityId))
            {
                _logger.LogWarning("Comments requested for missing {EntityType} {EntityId}", entityType, entityId);
                return new List<CommentResponse>();
            }

            var comments = await _db.Comments
                .AsNoTracking()
                .Include(c => c.MentionComments)
                .Where(c => c.EntityId == entityId && c.EntityType == entityType && !c.IsDeleted)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(c => ToResponse(c)).ToList();
        }

        public async Task<CommentResponse?> UpdateComment(Guid commentId, CommentRequest commentRequest)
        {
            var comment = await _db.Comments
                .Include(c => c.MentionComments)
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} was not found for update", commentId);
                return null;
            }

            var mentionedUserIds = NormalizeMentionUserIds(commentRequest.MentionUserIds);
            await ValidateMentionedUsersAsync(mentionedUserIds);

            comment.Body = NormalizeBody(commentRequest.Body);
            comment.UpdatedAt = DateTime.UtcNow;
            comment.UpdatedByUserId = currentUser.UserId;

            var mentionsToRemove = comment.MentionComments
                .Where(c => !mentionedUserIds.Contains(c.MentionedUserId))
                .ToList();

            _db.MentionComments.RemoveRange(mentionsToRemove);

            var existingMentionUserIds = comment.MentionComments
                .Select(c => c.MentionedUserId)
                .ToHashSet();

            var mentionsToAdd = mentionedUserIds
                .Where(userId => !existingMentionUserIds.Contains(userId))
                .Select(userId => new MentionComment
                {
                    Id = Guid.NewGuid(),
                    CommentId = comment.Id,
                    MentionedUserId = userId
                })
                .ToList();

            await _db.MentionComments.AddRangeAsync(mentionsToAdd);

            await _db.SaveChangesAsync();

            _logger.LogInformation("Comment {CommentId} updated by user {UserId}", commentId, currentUser.UserId);
            return ToResponse(comment, mentionedUserIds);
        }

        public async Task<bool> DeleteComment(Guid commentId)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} was not found for delete", commentId);
                return false;
            }

            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            comment.DeletedByUserId = currentUser.UserId;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Comment {CommentId} deleted by user {UserId}", commentId, currentUser.UserId);
            return true;
        }

        private async Task<bool> ExistEntityAsync(EntityEnums entity, Guid entityId)
        {
            return entity switch
            {
                EntityEnums.Task => await _db.Tasks.AnyAsync(t => t.Id == entityId && !t.IsDeleted),
                EntityEnums.Feature => await _db.Features.AnyAsync(f => f.Id == entityId && !f.IsDeleted),
                EntityEnums.BacklogItem => await _db.BacklogItems.AnyAsync(b => b.Id == entityId && !b.IsDeleted),
                EntityEnums.Project => await _db.Projects.AnyAsync(p => p.ProjectId == entityId && !p.IsDeleted),
                _ => false
            };
        }

        private async Task ValidateMentionedUsersAsync(List<Guid> mentionedUserIds)
        {
            if (!mentionedUserIds.Any())
            {
                return;
            }

            var existingUserCount = await _db.Users
                .CountAsync(u => mentionedUserIds.Contains(u.UserId));

            if (existingUserCount != mentionedUserIds.Count)
            {
                _logger.LogWarning("Comment validation failed because one or more mentioned users were not found");
                throw new ArgumentException("One or more mentioned users were not found.");
            }
        }

        private bool CanModify(Comment comment)
        {
            return currentUser.IsAdmin || comment.CreatedByUserId == currentUser.UserId;
        }

        private static string NormalizeBody(string body)
        {
            body = body?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("Comment body is required.");
            }

            return body;
        }

        private static List<Guid> NormalizeMentionUserIds(IEnumerable<Guid>? mentionedUserIds)
        {
            return mentionedUserIds?
                .Where(userId => userId != Guid.Empty)
                .Distinct()
                .ToList() ?? new List<Guid>();
        }

        private static CommentResponse ToResponse(Comment comment, ICollection<Guid>? mentionedUserIds = null)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                EntityId = comment.EntityId,
                EntityType = comment.EntityType,
                CreatedByUserId = comment.CreatedByUserId,
                MentionedUserIds = mentionedUserIds ?? comment.MentionComments.Select(cm => cm.MentionedUserId).ToList()
            };
        }
    }
}
