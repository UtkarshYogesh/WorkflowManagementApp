using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs.Auth;
using TaskManagement.Api.Helpers;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        public readonly AppDbContext appDbContext;
        public readonly PasswordHasher<User> passwordHasher;
        public readonly JwtHelper jwtHelper;
        public AuthService(AppDbContext _appDbContext, JwtHelper _jwtHelper)
        {
            appDbContext = _appDbContext;
            passwordHasher = new PasswordHasher<User>();
            jwtHelper = _jwtHelper;

        }

        public async Task RegisterUser(RegisterUserRequest userRequest)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = userRequest.Username,
                Email = userRequest.Email,
                Role = "Member",
                CreatedAt = DateTime.UtcNow

            };

            user.PasswordHash = passwordHasher.HashPassword(user, userRequest.Password);
            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();

        }

        public async Task<AuthResponse> LoginUser(LoginUserRequest loginUserRequest)
        {
            var user = appDbContext.Users.FirstOrDefault(u => u.Email == loginUserRequest.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password.");
            }
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserRequest.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid email or password.");
            }

           return await GenerateTokens(user);
            // Return the token to the client (e.g., in a response object)
        }

        public async Task<AuthResponse> Refresh(string refreshToken)
        {
            var token = await appDbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || token.IsRevoked || token.Expires < DateTime.UtcNow)
                throw new Exception("Invalid refresh token");

            token.IsRevoked = true;

            return await GenerateTokens(token.User);
        }

        private async Task<AuthResponse> GenerateTokens(User user)
        {
            var accessToken = jwtHelper.GenerateAccessToken(user);
            var refreshToken = jwtHelper.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.UserId
            };

            appDbContext.RefreshTokens.Add(refreshTokenEntity);
            await appDbContext.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
