using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.User;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class UserService : IUserInterface
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
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

            user.Role = role;
            await _db.SaveChangesAsync();
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
