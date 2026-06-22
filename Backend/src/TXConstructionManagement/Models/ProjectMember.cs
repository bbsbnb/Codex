namespace TXConstructionManagement.Models;

public class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public Position Position { get; set; }
    public bool IsPrimary { get; set; } = false;
    public DateTime AssignedAt { get; set; } = DateTime.Now;
    public Project? Project { get; set; }
    public User? User { get; set; }
}

public enum Position
{
    ProjectManager = 0,
    DeputyProjectManager = 1,
    ChiefEngineer = 2,
    ProductionManager = 3,
    ConstructionWorker = 4,
    QualityInspector = 5,
    Technician = 6,
    SafetyOfficer = 7,
    MaterialManager = 8,
    DocumentController = 9
}