namespace TXConstructionManagement.Models;

public class WarningRecord
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public WarningType Type { get; set; }
    public WarningLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsResolved { get; set; } = false;
    public DateTime TriggeredAt { get; set; } = DateTime.Now;
    public DateTime? ResolvedAt { get; set; }
    public Project? Project { get; set; }
}

public enum WarningType
{
    ClaimDeadline = 0,
    OverBudget = 1,
    OverContract = 2,
    MaterialDeviation = 3,
    PlanOverdue = 4,
    DocumentMissing = 5
}

public enum WarningLevel
{
    Info = 0,
    Warning = 1,
    Critical = 2
}