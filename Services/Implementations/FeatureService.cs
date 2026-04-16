using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Feature;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class FeatureService : IFeatureInterface
    {
        public readonly AppDbContext _db;
        public FeatureService(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        public async Task<List<FeatureResponse>> GetAllFeatures()
        {
            var features = await _db.Features
                .Select(f => new FeatureResponse
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Status = f.Status,
                    CreatedAt = f.CreatedAt,
                    ProjectId = f.ProjectId,
                    AssignedToUserId = f.AssignedToUserId
                })
                .ToListAsync();

            return features;
        }

        public async Task<FeatureResponse> AddFeatureToProject(FeatureRequest featureRequest, Guid projectId)
        {
            var feature = new Feature
            {
                Name = featureRequest.Name,
                Description = featureRequest.Description,
                AssignedToUserId = featureRequest.AssignedToUserId,
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                Status = "Planned",
            };

            await _db.Features.AddAsync(feature);
            await _db.SaveChangesAsync();

            return new FeatureResponse
            {
                Id = feature.Id,
                Name = feature.Name,
                Description = feature.Description,
                Status = feature.Status,
                CreatedAt = feature.CreatedAt,
                ProjectId = feature.ProjectId,
                AssignedToUserId = feature.AssignedToUserId
            };
        }

        public async Task<List<FeatureResponse>> GetFeaturesByProjectId(Guid projectId)
        {
            var features = await _db.Features
                .Where(f => f.ProjectId == projectId)
                .Select(f => new FeatureResponse
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Status = f.Status,
                    CreatedAt = f.CreatedAt,
                    ProjectId = f.ProjectId,
                    AssignedToUserId = f.AssignedToUserId
                })
                .ToListAsync();
            return features;
        }

        public async Task<FeatureResponse> GetFeatureById(Guid featureId)
        {
            var feature = await _db.Features
                .FirstOrDefaultAsync(f => f.Id == featureId);
            if (feature == null) return null;
            return new FeatureResponse
            {
                Id = feature.Id,
                Name = feature.Name,
                Description = feature.Description,
                Status = feature.Status,
                CreatedAt = feature.CreatedAt,
                ProjectId = feature.ProjectId,
                AssignedToUserId = feature.AssignedToUserId
            };
        }

        public async Task<FeatureResponse> UpdatedFeatureStatus(Guid featureId, string newStatus)
        {
            var feature = await _db.Features.FirstOrDefaultAsync(f => f.Id == featureId);
            if (feature == null) return null;
            feature.Status = newStatus;
            await _db.SaveChangesAsync();

            return new FeatureResponse
            {
                Id = feature.Id,
                Name = feature.Name,
                Description = feature.Description,
                Status = feature.Status,
                CreatedAt = feature.CreatedAt,
                ProjectId = feature.ProjectId,
                AssignedToUserId = feature.AssignedToUserId
            };
        }

        public async Task<FeatureResponse> AddUserToFeature(Guid featureId, Guid userId)
        {
            var feature = await _db.Features.FirstOrDefaultAsync(f => f.Id == featureId);
            if (feature == null) return null;
            feature.AssignedToUserId = userId;
            await _db.SaveChangesAsync();
            return new FeatureResponse
            {
                Id = feature.Id,
                Name = feature.Name,
                Description = feature.Description,
                Status = feature.Status,
                CreatedAt = feature.CreatedAt,
                ProjectId = feature.ProjectId,
                AssignedToUserId = feature.AssignedToUserId
            };
        }

        public async Task DeleteFeature(Guid featureId)
        {
            var feature = await _db.Features.FirstOrDefaultAsync(f => f.Id == featureId);
            if (feature != null)
            {
                _db.Features.Remove(feature);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<FeatureResponse> UpdateFeature(Guid featureId, FeatureRequest featureRequest)
        {
            var feature = await _db.Features.FirstOrDefaultAsync(f => f.Id == featureId);
            if (feature != null)
            {
                feature.Name = featureRequest.Name;
                feature.Description = featureRequest.Description;
                feature.AssignedToUserId = featureRequest.AssignedToUserId;
                await _db.SaveChangesAsync();

                return new FeatureResponse
                {
                    Id = feature.Id,
                    Name = feature.Name,
                    Description = feature.Description,
                    Status = feature.Status,
                    CreatedAt = feature.CreatedAt,
                    ProjectId = feature.ProjectId,
                    AssignedToUserId = feature.AssignedToUserId
                };
            }
            return null;
        }
    }
}
