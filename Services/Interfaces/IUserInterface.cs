using TaskManagement.Api.DTOs.User;

namespace TaskManagement.Api.Services.Interfaces
{
    public interface IUserInterface
    {
        Task<List<UserResponse>> GetAllUsers();
    }
}
