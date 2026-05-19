using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Project;
using TaskManagement.Api.Services.Implementations;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Controllers.ProjectController
{
    [Authorize]
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        public readonly IProjectInterface projectService;
        public ProjectController(IProjectInterface _projectService)
        {
            projectService = _projectService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProject(ProjectRequest projectRequest)
        {

            var project = await projectService.AddProjectAsync(projectRequest);
            return Ok(project);

        }

        [HttpGet( "{projectId}")]
        public async Task<IActionResult> GetProjectById(Guid projectId)
        {
            var project = await projectService.GetProjectById(projectId);
            return Ok(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] ProjectRequest projectRequest)
        {
            var project = await projectService.UpdateProjectAsync(projectId, projectRequest);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{projectId}/status")]
        public async Task<IActionResult> UpdateProjectStatus(Guid projectId, [FromBody] string status)
        {
            var project = await projectService.UpdateProjectStatusAsync(projectId, status);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProjectById(Guid projectId )
        {
            var deleted = await projectService.DeleteProjectAsync(projectId);
            if (!deleted) return Forbid();
            return NoContent();
        }




    }
}
