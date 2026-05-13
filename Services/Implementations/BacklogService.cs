using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Backlog;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class BacklogService : IBacklogInterface
    {
        public readonly AppDbContext _db;
        private readonly ICurrentUserService currentUser;

        public BacklogService(AppDbContext appDbContext, ICurrentUserService currentUser)
        {
            _db = appDbContext;
            this.currentUser = currentUser;
        }

        public async Task<List<BacklogResponse>> GetAllBacklogs()
        {
            return await _db.BacklogItems
                .Where(b => !b.IsDeleted && !b.Feature.IsDeleted && !b.Feature.Project.IsDeleted)
                .Select(b => ToResponse(b))
                .ToListAsync();
        }

        public async Task<BacklogResponse> AddBacklogToFeature(Guid featureId, BacklogRequest backlogRequest)
        {
            var backlogItem = new BacklogItem
            {
                Id = Guid.NewGuid(),
                Title = backlogRequest.Title,
                Description = backlogRequest.Description,
                Status = "Planned",
                Priority = NormalizePriority(backlogRequest.Priority),
                Type = NormalizeType(backlogRequest.Type),
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUser.UserId,
                FeatureId = featureId,
                AssignedToUserId = backlogRequest.AssignedToUserId
            };

            _db.BacklogItems.Add(backlogItem);
            await _db.SaveChangesAsync();

            return ToResponse(backlogItem);
        }

        public async Task<List<BacklogResponse>> GetBacklogsByFeatureId(Guid featureId)
        {
            return await _db.BacklogItems
                .Where(b => b.FeatureId == featureId && !b.IsDeleted && !b.Feature.IsDeleted)
                .Select(b => ToResponse(b))
                .ToListAsync();
        }

        public async Task<BacklogResponse> GetBacklogById(Guid backlogId)
        {
            var backlogItem = await _db.BacklogItems
                .FirstOrDefaultAsync(b => b.Id == backlogId && !b.IsDeleted && !b.Feature.IsDeleted);

            return backlogItem == null ? null : ToResponse(backlogItem);
        }

        public async Task<BacklogResponse> UpdateBacklog(Guid backlogId, BacklogRequest backlogRequest)
        {
            var backlogItem = await _db.BacklogItems.FirstOrDefaultAsync(b => b.Id == backlogId && !b.IsDeleted);
            if (backlogItem == null) return null;

            backlogItem.Title = backlogRequest.Title;
            backlogItem.Description = backlogRequest.Description;
            backlogItem.Priority = NormalizePriority(backlogRequest.Priority);
            backlogItem.Type = NormalizeType(backlogRequest.Type);
            backlogItem.AssignedToUserId = backlogRequest.AssignedToUserId;
            SetUpdated(backlogItem);
            await _db.SaveChangesAsync();

            return ToResponse(backlogItem);
        }

        public async Task<bool> DeleteBacklog(Guid backlogId)
        {
            var backlogItem = await _db.BacklogItems.FirstOrDefaultAsync(b => b.Id == backlogId && !b.IsDeleted);
            if (backlogItem == null || !CanDelete(backlogItem)) return false;

            backlogItem.IsDeleted = true;
            backlogItem.DeletedByUserId = currentUser.UserId;
            backlogItem.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<BacklogResponse> UpdateBacklogStatus(Guid backlogId, string status)
        {
            var backlogItem = await _db.BacklogItems.FirstOrDefaultAsync(b => b.Id == backlogId && !b.IsDeleted);
            if (backlogItem == null) return null;

            backlogItem.Status = status;
            SetUpdated(backlogItem);
            await _db.SaveChangesAsync();

            return ToResponse(backlogItem);
        }

        public async Task<BacklogResponse> AssignBacklogToUser(Guid backlogId, Guid userId)
        {
            var backlogItem = await _db.BacklogItems.FirstOrDefaultAsync(b => b.Id == backlogId && !b.IsDeleted);
            if (backlogItem == null) return null;

            backlogItem.AssignedToUserId = userId;
            SetUpdated(backlogItem);
            await _db.SaveChangesAsync();

            return ToResponse(backlogItem);
        }

        private bool CanDelete(BacklogItem backlogItem)
        {
            return currentUser.IsAdmin || backlogItem.CreatedByUserId == currentUser.UserId;
        }

        private void SetUpdated(BacklogItem backlogItem)
        {
            backlogItem.UpdatedByUserId = currentUser.UserId;
            backlogItem.UpdatedAt = DateTime.UtcNow;
        }

        private static BacklogResponse ToResponse(BacklogItem backlogItem)
        {
            return new BacklogResponse
            {
                Id = backlogItem.Id,
                Title = backlogItem.Title,
                Description = backlogItem.Description,
                Status = backlogItem.Status,
                Priority = backlogItem.Priority,
                Type = backlogItem.Type,
                CreatedAt = backlogItem.CreatedAt,
                CreatedByUserId = backlogItem.CreatedByUserId,
                FeatureId = backlogItem.FeatureId,
                AssignedToUserId = backlogItem.AssignedToUserId
            };
        }

        private static string NormalizePriority(string priority)
        {
            return priority?.Trim().ToUpperInvariant() switch
            {
                "P1" => "P1",
                "P2" => "P2",
                _ => "P3"
            };
        }

        private static string NormalizeType(string type)
        {
            return type?.Trim() switch
            {
                "Bug" => "Bug",
                "Improvement" => "Improvement",
                "Technical" => "Technical",
                _ => "Story"
            };
        }
    }
}
