namespace TXConstructionManagement.Models;

public class BusinessFlow
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public FlowType FlowType { get; set; }
    public string FlowNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int InitiatorId { get; set; }
    public DateTime InitiatedAt { get; set; } = DateTime.Now;
    public FlowStatus Status { get; set; } = FlowStatus.Draft;
    public int CurrentNodeId { get; set; }
    public decimal? Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
    public Project? Project { get; set; }
    public ICollection<ApprovalRecord>? Approvals { get; set; }
}

public enum FlowType
{
    ContactOrder = 0,
    PriceConfirmation = 1,
    Visa = 2,
    Claim = 3,
    DesignChange = 4,
    MonthlyValuation = 5,
    MaterialSettlement = 6,
    ConsumptionVerification = 7,
    Bidding = 8,
    ContractIssue = 9,
    MaterialIssue = 10,
    PaymentCollection = 11,
    QualityInspection = 12,
    SafetyInspection = 13,
    ProgressInspection = 14,
    CivilConstruction = 15,
    DocumentIssue = 16,
    ReviewMeeting = 17,
    SecondaryPlanning = 18
}

public enum FlowStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Archived = 4,
    Cancelled = 5
}