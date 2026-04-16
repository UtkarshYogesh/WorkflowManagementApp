using TaskManagement.Api.DTOs.Backlog;

namespace TaskManagement.Api.Services.Interfaces
{
    public interface IBacklogInterface
    {
        Task<List<BacklogResponse>> GetAllBacklogs();
        Task<BacklogResponse> AddBacklogToFeature(Guid featureId, BacklogRequest backlogRequest);
        Task<List<BacklogResponse>> GetBacklogsByFeatureId(Guid featureId);

        Task<BacklogResponse> GetBacklogById(Guid backlogId);

        Task<BacklogResponse> UpdateBacklog(Guid backlogId, BacklogRequest backlogRequest);

        Task DeleteBacklog(Guid backlogId);

        Task<BacklogResponse> UpdateBacklogStatus(Guid backlogId, string status);

        Task<BacklogResponse> AssignBacklogToUser(Guid backlogId, Guid userId);


    }
}
