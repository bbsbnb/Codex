using TXConstructionManagement.Models;

namespace TXConstructionManagement.Services.Interfaces;

public interface IWorkflowEngineService
{
    Task<BusinessFlow> CreateFlowAsync(BusinessFlow flow);
    Task<ApprovalRecord?> SubmitApprovalAsync(int flowId, int approverId, ApprovalAction action, string comment);
    Task<BusinessFlow?> GetFlowByIdAsync(int flowId);
    Task<List<BusinessFlow>> GetPendingApprovalsAsync(int userId);
    Task<List<BusinessFlow>> GetFlowsByProjectAsync(int projectId);
    Task<List<BusinessFlow>> GetFlowsByTypeAsync(int projectId, FlowType flowType);
    Task<bool> ArchiveFlowAsync(int flowId);
    List<NodeDefinition>? GetStandardWorkflow(FlowType flowType);
    Dictionary<FlowType, List<NodeDefinition>> GetAllStandardWorkflows();
}

public class NodeDefinition
{
    public int NodeIndex { get; set; }
    public string Position { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}