using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Project;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class ProjectService : IProjectInterface
    {
        public readonly AppDbContext _db;
        public ProjectService(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }
        public async Task<List<ProjectResponse>> GetAllProjectsAsync()
        {
            return await _db.Projects
                .Select(p => new ProjectResponse
                {
                    ProjectId = p.ProjectId,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ProjectResponse> AddProjectAsync(ProjectRequest request)
        {
            var project = new Project
            {
                ProjectId = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return new ProjectResponse
            {
                ProjectId = project.ProjectId,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            };
        }

        public async Task<ProjectResponse> GetProjectById(Guid projectId)
        {
            var project = await _db.Projects
                .Include(p => p.Features) // Include related features if needed
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
            if (project == null) return null;
            return new ProjectResponse
            {
                ProjectId = project.ProjectId,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            };
        }

        public async Task DeleteProjectAsync(Guid projectId)
        {
            var project = await _db.Projects.FindAsync(projectId);
            if (project != null)
            {
                _db.Projects.Remove(project);
                await _db.SaveChangesAsync();
            }
        }


    }
}
