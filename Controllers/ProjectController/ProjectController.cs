using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Project;
using TaskManagement.Api.Services.Implementations;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Controllers.ProjectController
{
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

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProjectById(Guid projectId )
        {
            await projectService.DeleteProjectAsync(projectId);
            return NoContent();
        }




    }
}
