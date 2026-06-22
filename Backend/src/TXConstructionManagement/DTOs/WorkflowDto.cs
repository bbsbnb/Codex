using TXConstructionManagement.Models;

namespace TXConstructionManagement.DTOs;

public class CreateFlowRequest
{
    public int ProjectId { get; set; }
    public FlowType FlowType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? Amount { get; set; }

    // 工联单专用
    public string? ContactParty { get; set; }
    // 认质认价专用
    public string? MaterialName { get; set; }
    public string? MaterialSpec { get; set; }
    public string? Brand { get; set; }
    public decimal? Quantity { get; set; }
    // 签证专用
    public string? VisaType { get; set; }
    // 索赔专用
    public DateTime? ClaimOccurrenceDate { get; set; }
    // 设计变更专用
    public string? ChangeNoticeNo { get; set; }
}

public class FlowDetailResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public FlowType FlowType { get; set; }
    public string FlowTypeName { get; set; } = string.Empty;
    public string FlowNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public FlowStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int CurrentNodeId { get; set; }
    public decimal? Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<ApprovalSummary>? Approvals { get; set; }
    public List<NodeDefinition>? WorkflowNodes { get; set; }
    public NodeDefinition? CurrentNode { get; set; }
}

public class SubmitApprovalRequest
{
    public ApprovalAction Action { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class FlowResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public FlowType FlowType { get; set; }
    public string FlowNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public FlowStatus Status { get; set; }
    public int CurrentNodeId { get; set; }
    public decimal? Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<ApprovalSummary>? Approvals { get; set; }
}

public class ApprovalSummary
{
    public int Id { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public int NodeIndex { get; set; }
    public ApprovalAction Action { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
}