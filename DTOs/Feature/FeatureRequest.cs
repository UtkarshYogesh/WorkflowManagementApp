namespace TaskManagement.Api.DTOs.Feature
{
    public class FeatureRequest
    {
        public string Name { get; set; } 
        public string Description { get; set; }
        public string Priority { get; set; } = "P3";

        public Guid? AssignedToUserId { get; set; }


    }
}
