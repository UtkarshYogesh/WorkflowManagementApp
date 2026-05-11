using System.Security.Claims;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var userId) ? userId : Guid.Empty;
            }
        }

        public string Role =>
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        public bool IsAdmin => string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);
    }
}
