namespace TXConstructionManagement.Models;

public class InspectionRecord
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public InspectionType Type { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public string IssueCategory { get; set; } = string.Empty;
    public int InspectorId { get; set; }
    public DateTime InspectionDate { get; set; } = DateTime.Now;
    public string RectificationRequirement { get; set; } = string.Empty;
    public DateTime? RectificationDeadline { get; set; }
    public InspectionStatus Status { get; set; } = InspectionStatus.Open;
    public string Result { get; set; } = string.Empty;
    public Project? Project { get; set; }
}

public enum InspectionType
{
    Quality = 0,
    Safety = 1,
    Progress = 2,
    CivilConstruction = 3,
    Contract = 4,
    Material = 5,
    Bidding = 6,
    Document = 7
}

public enum InspectionStatus
{
    Open = 0,
    InProgress = 1,
    Closed = 2,
    Overdue = 3
}