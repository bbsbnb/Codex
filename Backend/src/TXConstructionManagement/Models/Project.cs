namespace TXConstructionManagement.Models;

public class Project
{
    public int Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public decimal ContractAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ExpectedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public string Address { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public ICollection<ProjectMember>? Members { get; set; }
    public ICollection<PrimaryDocument>? Documents { get; set; }
    public ICollection<BusinessFlow>? Flows { get; set; }
    public ICollection<MonthlyPlan>? Plans { get; set; }
    public ICollection<InspectionRecord>? Inspections { get; set; }
    public ICollection<PaymentRecord>? Payments { get; set; }
}

public enum ProjectStatus
{
    Active = 0,
    Suspended = 1,
    Completed = 2
}