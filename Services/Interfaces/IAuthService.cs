using TaskManagement.Api.DTOs.Auth;

namespace TaskManagement.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterUser(RegisterUserRequest userRequest);
        Task<AuthResponse> LoginUser(LoginUserRequest loginUserRequest);

        Task<AuthResponse> Refresh(string refreshToken);
    }
}
