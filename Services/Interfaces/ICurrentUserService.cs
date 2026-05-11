namespace TaskManagement.Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Role { get; }
        bool IsAdmin { get; }
    }
}
