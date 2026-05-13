using TaskManagement.Api.Models.Enums;

namespace TaskManagement.Api.Helpers
{
    public static class StatusHelper
    {
        public const string WaitingForInformation = "Waiting for Information";
        public const string InProgress = "In Progress";

        public static string NormalizeProjectStatus(string? status)
        {
            return status?.Trim() switch
            {
                "Committed" => nameof(ProjectStatus.Committed),
                "Done" => nameof(ProjectStatus.Done),
                "Blocked" => nameof(ProjectStatus.Blocked),
                WaitingForInformation => WaitingForInformation,
                _ => nameof(ProjectStatus.New)
            };
        }

        public static string NormalizeFeatureStatus(string? status)
        {
            return status?.Trim() switch
            {
                "Committed" => nameof(FeatureStatus.Committed),
                "Done" => nameof(FeatureStatus.Done),
                "Blocked" => nameof(FeatureStatus.Blocked),
                "Close" => nameof(FeatureStatus.Close),
                _ => nameof(FeatureStatus.Planned)
            };
        }

        public static string NormalizeBacklogStatus(string? status)
        {
            return status?.Trim() switch
            {
                "Committed" or "Comitted" => nameof(BacklogStatus.Committed),
                "Done" => nameof(BacklogStatus.Done),
                "Resolved" => nameof(BacklogStatus.Resolved),
                "Blocked" => nameof(BacklogStatus.Blocked),
                WaitingForInformation => WaitingForInformation,
                _ => nameof(BacklogStatus.New)
            };
        }

        public static string NormalizeTaskStatus(string? status)
        {
            return status?.Trim() switch
            {
                InProgress => InProgress,
                "Done" => nameof(TaskWorkStatus.Done),
                _ => nameof(TaskWorkStatus.Todo)
            };
        }
    }
}
