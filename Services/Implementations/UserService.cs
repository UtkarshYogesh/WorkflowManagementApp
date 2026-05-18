using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.User;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class UserService : IUserInterface
    {
        private readonly AppDbContext _db;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext db, ILogger<UserService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<UserResponse>> GetAllUsers()
        {
            return await _db.Users
                .OrderBy(u => u.Username)
                .Select(u => new UserResponse
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();
        }

        public async Task<UserResponse> UpdateUserRole(string role, Guid userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.UserId == userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} was not found for role update", userId);
                return null;
            }

            user.Role = role;
            await _db.SaveChangesAsync();
            _logger.LogInformation("User {UserId} role updated to {Role}", userId, role);
            return new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
