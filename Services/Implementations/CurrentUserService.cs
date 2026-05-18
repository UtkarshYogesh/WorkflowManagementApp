using System.Security.Claims;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Services.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Guid UserId
        {
            get
            {
                var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(value, out var userId))
                {
                    return userId;
                }

                _logger.LogDebug("Current request does not have a valid user id claim");
                return Guid.Empty;
            }
        }

        public string Role =>
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        public bool IsAdmin => string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);
    }
}
