namespace TXConstructionManagement.DTOs;

public class CreateProjectRequest
{
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public decimal ContractAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ExpectedEndDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateProjectRequest
{
    public string? ProjectName { get; set; }
    public decimal? ContractAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ExpectedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public ProjectStatus? Status { get; set; }
    public string? Address { get; set; }
    public string? ClientName { get; set; }
    public string? Description { get; set; }
}

public class ProjectResponse
{
    public int Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public decimal ContractAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ExpectedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public string Address { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
}