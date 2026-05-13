namespace TaskManagement.Api.Models.Enums
{
    public enum ProjectStatus
    {
        New,
        Committed,
        Done,
        Blocked,
        WaitingForInformation
    }

    public enum FeatureStatus
    {
        Planned,
        Committed,
        Done,
        Blocked,
        Close
    }

    public enum BacklogStatus
    {
        New,
        Committed,
        Done,
        Resolved,
        Blocked,
        WaitingForInformation
    }

    public enum TaskWorkStatus
    {
        Todo,
        InProgress,
        Done
    }
}
