using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Project;

namespace TaskManagement.Api.Services.Interfaces
{
    public interface IProjectInterface
    {
        Task<List<ProjectResponse>> GetAllProjectsAsync();
        Task<ProjectResponse> AddProjectAsync(ProjectRequest request);

        Task<ProjectResponse> GetProjectById(Guid projectId);

        Task DeleteProjectAsync(Guid projectId);

    }
}
