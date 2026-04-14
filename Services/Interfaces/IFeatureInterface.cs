using TaskManagement.Api.DTOs.Feature;

namespace TaskManagement.Api.Services.Interfaces
{
    public interface IFeatureInterface
    {
        Task<FeatureResponse> AddFeatureToProject(FeatureRequest featureRequest, Guid projectId);

        Task<List<FeatureResponse>> GetFeaturesByProjectId(Guid projectId);
        Task<FeatureResponse> GetFeatureById(Guid featureId);

        Task<FeatureResponse> UpdatedFeatureStatus(Guid featureId, string newStatus);
        Task<FeatureResponse> AddUserToFeature(Guid featureId, Guid userId);
        Task DeleteFeature(Guid featureId);
        Task<FeatureResponse> UpdateFeature(Guid featureId, FeatureRequest featureRequest);

    }
}
