using TXConstructionManagement.Models;

namespace TXConstructionManagement.DTOs;

public class CreateReviewMeetingRequest
{
    public int ProjectId { get; set; }
    public string MeetingTitle { get; set; } = string.Empty;
    public string Participants { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Conclusions { get; set; } = string.Empty;
    public string ActionItems { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
}

public class ReviewMeetingResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string MeetingTitle { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public string Participants { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Conclusions { get; set; } = string.Empty;
    public string ActionItems { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public ReviewMeetingStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateNotificationRequest
{
    public int? UserId { get; set; }
    public int? ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationLevel Level { get; set; }
}

public class NotificationResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MonthlyAnalysisResponse
{
    public int ProjectCount { get; set; }
    public int ActiveProjectCount { get; set; }
    public int MonthlyFlowCount { get; set; }
    public decimal TotalSettledAmount { get; set; }
    public int PendingApprovalCount { get; set; }
    public int OpenWarningCount { get; set; }
    public int OpenInspectionCount { get; set; }
    public decimal TotalPaymentCollected { get; set; }
    public decimal TotalPaymentPending { get; set; }
    public int ReviewMeetingCount { get; set; }
    public List<FlowTypeSummary> FlowTypeSummaries { get; set; } = new();
}

public class FlowTypeSummary
{
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CreateWorkflowTemplateRequest
{
    public string TemplateName { get; set; } = string.Empty;
    public FlowType FlowType { get; set; }
    public string NodesJson { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}