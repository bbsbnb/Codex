using TXConstructionManagement.Models;

namespace TXConstructionManagement.DTOs;

public class CreateInspectionRequest
{
    public int ProjectId { get; set; }
    public InspectionType Type { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public string IssueCategory { get; set; } = string.Empty;
    public string RectificationRequirement { get; set; } = string.Empty;
    public DateTime? RectificationDeadline { get; set; }
}

public class UpdateInspectionRequest
{
    public string? Result { get; set; }
    public InspectionStatus? Status { get; set; }
    public string? RectificationRequirement { get; set; }
    public DateTime? RectificationDeadline { get; set; }
}

public class InspectionResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public InspectionType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string IssueDescription { get; set; } = string.Empty;
    public string IssueCategory { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public string RectificationRequirement { get; set; } = string.Empty;
    public DateTime? RectificationDeadline { get; set; }
    public InspectionStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
}