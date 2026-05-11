using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.User;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Controllers.UserController
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserInterface userService;

        public UserController(IUserInterface _userService)
        {
            userService = _userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await userService.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{userId}")]
        public async Task<IActionResult> ChangeUserRole(Guid userId, [FromBody] string role)
        {
            var user = await userService.UpdateUserRole(role, userId);
            return Ok(user);
        }
    }
}
