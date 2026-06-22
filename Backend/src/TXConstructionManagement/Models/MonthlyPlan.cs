namespace TXConstructionManagement.Models;

public class MonthlyPlan
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public PlanType PlanType { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public PlanStatus Status { get; set; } = PlanStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Project? Project { get; set; }
}

public enum PlanType
{
    ConstructionSchedule = 0,
    MaterialPurchase = 1,
    CapitalPlan = 2,
    SecondaryOperation = 3
}

public enum PlanStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Overdue = 3
}