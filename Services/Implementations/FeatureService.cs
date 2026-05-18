using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Feature;
using TaskManagement.Api.Helpers;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class FeatureService : IFeatureInterface
    {
        public readonly AppDbContext _db;
        private readonly ICurrentUserService currentUser;
        private readonly ILogger<FeatureService> _logger;

        public FeatureService(AppDbContext appDbContext, ICurrentUserService currentUser, ILogger<FeatureService> logger)
        {
            _db = appDbContext;
            this.currentUser = currentUser;
            _logger = logger;
        }

        public async Task<List<FeatureResponse>> GetAllFeatures()
        {
            return await _db.Features
                .Where(f => !f.IsDeleted && !f.Project.IsDeleted)
                .Select(f => ToResponse(f))
                .ToListAsync();
        }

        public async Task<FeatureResponse> AddFeatureToProject(FeatureRequest featureRequest, Guid projectId)
        {
            var feature = new Feature
            {
                Name = featureRequest.Name,
                Description = featureRequest.Description,
                Priority = NormalizePriority(featureRequest.Priority),
                AssignedToUserId = featureRequest.AssignedToUserId,
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUser.UserId,
                Status = StatusHelper.NormalizeFeatureStatus(null),
            };

            await _db.Features.AddAsync(feature);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Feature {FeatureId} added to project {ProjectId} by user {UserId}", feature.Id, projectId, currentUser.UserId);
            return ToResponse(feature);
        }

        public async Task<List<FeatureResponse>> GetFeaturesByProjectId(Guid projectId)
        {
            return await _db.Features
                .Where(f => f.ProjectId == projectId && !f.IsDeleted && !f.Project.IsDeleted)
                .Select(f => ToResponse(f))
                .ToListAsync();
        }

        public async Task<FeatureResponse> GetFeatureById(Guid featureId)
        {
            var feature = await _db.Features
                .FirstOrDefaultAsync(f => f.Id == featureId && !f.IsDeleted && !f.Project.IsDeleted);

            return feature == null ? null : ToResponse(feature);
        }

        public async Task<FeatureResponse> UpdatedFeatureStatus(Guid featureId, string newStatus)
        {
            var feature = await _db.Features.FirstOrDefaultAsync(f => f.Id == featureId && !f.IsDeleted);
            if (feature == null)
            {
                _logger.LogWarning("Feature {FeatureId} was not found for status update", featureId);
                return null;
            }

            feature.Status = StatusHelper.NormalizeFeatureStatus(newStatus);
            SetUpdated(feature);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Feature {FeatureId} status updated to {Status} by user {UserId}", featureId, feature.Status, currentUser.UserId);
            return ToResponse(feature);
        }

        public async Task<FeatureResponse> AddUserToFeature(Guid featureId, Guid userId)
        {
            var feature = await _db.Features.FirstOrDefaultAsync(f => f.Id == featureId && !f.IsDeleted);
            if (feature == null)
            {
                _logger.LogWarning("Feature {FeatureId} was not found for assignment", featureId);
                return null;
            }

            feature.AssignedToUserId = userId;
            SetUpdated(feature);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Feature {FeatureId} assigned to user {AssignedUserId} by user {UserId}", featureId, userId, currentUser.UserId);
            return ToResponse(feature);
        }

        public async Task<bool> DeleteFeature(Guid featureId)
        {
            var feature = await _db.Features.FirstOrDefaultAsync(f => f.Id == featureId && !f.IsDeleted);
            if (feature == null)
            {
                _logger.LogWarning("Feature {FeatureId} was not found for delete", featureId);
                return false;
            }

            if (!CanDelete(feature))
            {
                _logger.LogWarning("User {UserId} attempted to delete feature {FeatureId} without permission", currentUser.UserId, featureId);
                return false;
            }

            feature.IsDeleted = true;
            feature.DeletedByUserId = currentUser.UserId;
            feature.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Feature {FeatureId} deleted by user {UserId}", featureId, currentUser.UserId);
            return true;
        }

        public async Task<FeatureResponse> UpdateFeature(Guid featureId, FeatureRequest featureRequest)
        {
            var feature = await _db.Features.FirstOrDefaultAsync(f => f.Id == featureId && !f.IsDeleted);
            if (feature == null)
            {
                _logger.LogWarning("Feature {FeatureId} was not found for update", featureId);
                return null;
            }

            feature.Name = featureRequest.Name;
            feature.Description = featureRequest.Description;
            feature.Priority = NormalizePriority(featureRequest.Priority);
            feature.AssignedToUserId = featureRequest.AssignedToUserId;
            SetUpdated(feature);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Feature {FeatureId} updated by user {UserId}", featureId, currentUser.UserId);
            return ToResponse(feature);
        }

        private bool CanDelete(Feature feature)
        {
            return currentUser.IsAdmin || feature.CreatedByUserId == currentUser.UserId;
        }

        private void SetUpdated(Feature feature)
        {
            feature.UpdatedByUserId = currentUser.UserId;
            feature.UpdatedAt = DateTime.UtcNow;
        }

        private static FeatureResponse ToResponse(Feature feature)
        {
            return new FeatureResponse
            {
                Id = feature.Id,
                Name = feature.Name,
                Description = feature.Description,
                Status = feature.Status,
                Priority = feature.Priority,
                CreatedAt = feature.CreatedAt,
                CreatedByUserId = feature.CreatedByUserId,
                ProjectId = feature.ProjectId,
                AssignedToUserId = feature.AssignedToUserId
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
    }
}
