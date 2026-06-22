namespace TXConstructionManagement.DTOs;

public class CreatePlanRequest
{
    public int ProjectId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public PlanType PlanType { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}

public class PlanResponse
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
    public PlanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}