namespace TXConstructionManagement.Models;

public class ApprovalRecord
{
    public int Id { get; set; }
    public int FlowId { get; set; }
    public int ApproverId { get; set; }
    public int NodeIndex { get; set; }
    public ApprovalAction Action { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; } = DateTime.Now;
    public BusinessFlow? Flow { get; set; }
    public User? Approver { get; set; }
}

public enum ApprovalAction
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Returned = 3
}