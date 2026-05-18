using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Project;
using TaskManagement.Api.Helpers;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class ProjectService : IProjectInterface
    {
        public readonly AppDbContext _db;
        private readonly ICurrentUserService currentUser;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(AppDbContext appDbContext, ICurrentUserService currentUser, ILogger<ProjectService> logger)
        {
            _db = appDbContext;
            this.currentUser = currentUser;
            _logger = logger;
        }

        public async Task<List<ProjectResponse>> GetAllProjectsAsync()
        {
            return await _db.Projects
                .Where(p => !p.IsDeleted)
                .Select(p => ToResponse(p))
                .ToListAsync();
        }

        public async Task<ProjectResponse> AddProjectAsync(ProjectRequest request)
        {
            var project = new Project
            {
                ProjectId = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Status = StatusHelper.NormalizeProjectStatus(request.Status),
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUser.UserId
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Project {ProjectId} created by user {UserId}", project.ProjectId, currentUser.UserId);
            return ToResponse(project);
        }

        public async Task<ProjectResponse> GetProjectById(Guid projectId)
        {
            var project = await _db.Projects
                .Include(p => p.Features)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId && !p.IsDeleted);

            return project == null ? null : ToResponse(project);
        }

        public async Task<ProjectResponse> UpdateProjectStatusAsync(Guid projectId, string status)
        {
            if (!currentUser.IsAdmin)
            {
                _logger.LogWarning("User {UserId} attempted to update project {ProjectId} status without admin role", currentUser.UserId, projectId);
                return null;
            }

            var project = await _db.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId && !p.IsDeleted);
            if (project == null)
            {
                _logger.LogWarning("Project {ProjectId} was not found for status update", projectId);
                return null;
            }

            project.Status = StatusHelper.NormalizeProjectStatus(status);
            project.UpdatedByUserId = currentUser.UserId;
            project.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Project {ProjectId} status updated to {Status} by user {UserId}", projectId, project.Status, currentUser.UserId);
            return ToResponse(project);
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId)
        {
            if (!currentUser.IsAdmin)
            {
                _logger.LogWarning("User {UserId} attempted to delete project {ProjectId} without admin role", currentUser.UserId, projectId);
                return false;
            }

            var project = await _db.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId && !p.IsDeleted);
            if (project == null)
            {
                _logger.LogWarning("Project {ProjectId} was not found for delete", projectId);
                return false;
            }

            project.IsDeleted = true;
            project.DeletedByUserId = currentUser.UserId;
            project.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Project {ProjectId} deleted by user {UserId}", projectId, currentUser.UserId);
            return true;
        }

        private static ProjectResponse ToResponse(Project project)
        {
            return new ProjectResponse
            {
                ProjectId = project.ProjectId,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                CreatedAt = project.CreatedAt,
                CreatedByUserId = project.CreatedByUserId
            };
        }
    }
}
