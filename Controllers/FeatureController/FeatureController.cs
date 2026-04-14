using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Feature;
using TaskManagement.Api.Services.Interfaces;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeatureInterface featureInterface;

        public FeaturesController(IFeatureInterface _featureInterface)
        {
            featureInterface = _featureInterface;
        }

        // ===============================
        // 🔹 Get features by project
        // GET: /api/projects/{projectId}/features
        // ===============================
        [HttpGet("projects/{projectId}/features")]
        public async Task<IActionResult> GetFeaturesByProjectId(Guid projectId)
        {
            var features = await featureInterface.GetFeaturesByProjectId(projectId);
            return Ok(features);
        }

        // ===============================
        // 🔹 Get feature by ID
        // GET: /api/features/{featureId}
        // ===============================
        [HttpGet("features/{featureId}")]
        public async Task<IActionResult> GetFeatureById(Guid featureId)
        {
            var feature = await featureInterface.GetFeatureById(featureId);
            if (feature == null) return NotFound();
            return Ok(feature);
        }

        // ===============================
        // 🔹 Create feature under project
        // POST: /api/projects/{projectId}/features
        // ===============================
        [HttpPost("projects/{projectId}/features")]
        public async Task<IActionResult> AddFeatureToProject(Guid projectId, [FromBody] FeatureRequest featureRequest)
        {
            var feature = await featureInterface.AddFeatureToProject(featureRequest, projectId);

            return CreatedAtAction(
                nameof(GetFeatureById),
                new { featureId = feature.Id },
                feature
            );
        }

        // ===============================
        // 🔹 Update feature
        // PUT: /api/features/{featureId}
        // ===============================
        [HttpPut("features/{featureId}")]
        public async Task<IActionResult> UpdateFeature(Guid featureId, [FromBody] FeatureRequest featureRequest)
        {
            var updatedFeature = await featureInterface.UpdateFeature(featureId, featureRequest);
            if (updatedFeature == null) return NotFound();
            return Ok(updatedFeature);
        }

        // ===============================
        // 🔹 Update feature status
        // PATCH: /api/features/{featureId}/status
        // ===============================
        [HttpPatch("features/{featureId}/status")]
        public async Task<IActionResult> UpdateFeatureStatus(Guid featureId, [FromBody] string newStatus)
        {
            var updatedFeature = await featureInterface.UpdatedFeatureStatus(featureId, newStatus);
            if (updatedFeature == null) return NotFound();
            return Ok(updatedFeature);
        }

        // ===============================
        // 🔹 Assign user to feature
        // PATCH: /api/features/{featureId}/assign/{userId}
        // ===============================
        [HttpPatch("features/{featureId}/assign/{userId}")]
        public async Task<IActionResult> AddUserToFeature(Guid featureId, Guid userId)
        {
            var updatedFeature = await featureInterface.AddUserToFeature(featureId, userId);
            if (updatedFeature == null) return NotFound();
            return Ok(updatedFeature);
        }

        // ===============================
        // 🔹 Delete feature
        // DELETE: /api/features/{featureId}
        // ===============================
        [HttpDelete("features/{featureId}")]
        public async Task<IActionResult> DeleteFeature(Guid featureId)
        {
           await featureInterface.DeleteFeature(featureId);
            return NoContent();
        }
    }
}