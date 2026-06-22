namespace TXConstructionManagement.Models;

public class WorkflowTemplate
{
    public int Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public FlowType FlowType { get; set; }
    public string NodesJson { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}