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
        public BacklogService(AppDbContext appDbContext) { _db = appDbContext; }

        public async Task<BacklogResponse> AddBacklogToFeature(Guid featureId, BacklogRequest backlogRequest)
        {
            var backlogItem = new BacklogItem
            {
                Id = Guid.NewGuid(),
                Title = backlogRequest.Title,
                Description = backlogRequest.Description,
                Status = "Planned",
                CreatedAt = DateTime.UtcNow,
                FeatureId = featureId,
                AssignedToUserId = backlogRequest.AssignedToUserId
            };
            _db.BacklogItems.Add(backlogItem);
            await _db.SaveChangesAsync();

            return new BacklogResponse
            {
                Id = backlogItem.Id,
                Title = backlogItem.Title,
                Description = backlogItem.Description,
                Status = backlogItem.Status,
                CreatedAt = backlogItem.CreatedAt,
                FeatureId = backlogItem.FeatureId,
                AssignedToUserId = backlogItem.AssignedToUserId
            };

        }

        public async Task<List<BacklogResponse>> GetBacklogsByFeatureId(Guid featureId)
        {
            var backlogItems = await _db.BacklogItems
                .Where(b => b.FeatureId == featureId)
                .Select(b => new BacklogResponse
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    FeatureId = b.FeatureId,
                    AssignedToUserId = b.AssignedToUserId
                })
                .ToListAsync();
            return backlogItems;
        }

        public async Task<BacklogResponse> GetBacklogById(Guid backlogId)
        {
            var backlogItem = await _db.BacklogItems
                .FirstOrDefaultAsync(b => b.Id == backlogId);
            if (backlogItem == null) return null;
            return new BacklogResponse
            {
                Id = backlogItem.Id,
                Title = backlogItem.Title,
                Description = backlogItem.Description,
                Status = backlogItem.Status,
                CreatedAt = backlogItem.CreatedAt,
                FeatureId = backlogItem.FeatureId,
                AssignedToUserId = backlogItem.AssignedToUserId
            };

        }

        public async Task<BacklogResponse> UpdateBacklog(Guid backlogId, BacklogRequest backlogRequest)
        {
            var backlogItem = await _db.BacklogItems.FirstOrDefaultAsync(b => b.Id == backlogId);
            if (backlogItem == null) return null;
            backlogItem.Title = backlogRequest.Title;
            backlogItem.Description = backlogRequest.Description;
            backlogItem.AssignedToUserId = backlogRequest.AssignedToUserId;
            await _db.SaveChangesAsync();
            return new BacklogResponse
            {
                Id = backlogItem.Id,
                Title = backlogItem.Title,
                Description = backlogItem.Description,
                Status = backlogItem.Status,
                CreatedAt = backlogItem.CreatedAt,
                FeatureId = backlogItem.FeatureId,
                AssignedToUserId = backlogItem.AssignedToUserId
            };
        }

        public async Task DeleteBacklog(Guid backlogId)
        {
            var backlogItem = await _db.BacklogItems.FirstOrDefaultAsync(b => b.Id == backlogId);
            if (backlogItem != null)
            {
                _db.BacklogItems.Remove(backlogItem);
                await _db.SaveChangesAsync();
            }


        }

        public async Task<BacklogResponse> UpdateBacklogStatus(Guid backlogId, string status)
        {
            var backlogItem = await _db.BacklogItems.FirstOrDefaultAsync(b => b.Id == backlogId);
            if (backlogItem == null) return null;
            backlogItem.Status = status;
            await _db.SaveChangesAsync();
            return new BacklogResponse
            {
                Id = backlogItem.Id,
                Title = backlogItem.Title,
                Description = backlogItem.Description,
                Status = backlogItem.Status,
                CreatedAt = backlogItem.CreatedAt,
                FeatureId = backlogItem.FeatureId,
                AssignedToUserId = backlogItem.AssignedToUserId
            };


        }

        public async Task<BacklogResponse> AssignBacklogToUser(Guid backlogId, Guid userId)
        {
            var backlogItem = await _db.BacklogItems.FirstOrDefaultAsync(b => b.Id == backlogId);
            if (backlogItem == null) return null;
            backlogItem.AssignedToUserId = userId;
            await _db.SaveChangesAsync();
            return new BacklogResponse
            {
                Id = backlogItem.Id,
                Title = backlogItem.Title,
                Description = backlogItem.Description,
                Status = backlogItem.Status,
                CreatedAt = backlogItem.CreatedAt,
                FeatureId = backlogItem.FeatureId,
                AssignedToUserId = backlogItem.AssignedToUserId
            };
        }


    }
}
