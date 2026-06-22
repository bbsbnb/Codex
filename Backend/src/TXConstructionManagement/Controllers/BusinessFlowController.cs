using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessFlowController : ControllerBase
{
    private readonly IWorkflowEngineService _wfService;

    public BusinessFlowController(IWorkflowEngineService wfService) { _wfService = wfService; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFlowRequest request)
    {
        if (request.ProjectId <= 0) return BadRequest(new { code = 400, message = "请选择项目" });
        if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest(new { code = 400, message = "标题不能为空" });

        var flow = new BusinessFlow
        {
            ProjectId = request.ProjectId,
            FlowType = request.FlowType,
            Title = request.Title,
            Description = request.Description,
            Amount = request.Amount
        };
        var result = await _wfService.CreateFlowAsync(flow);
        return Ok(new { code = 200, data = MapToResponse(result) });
    }

    [HttpPost("{flowId}/approve")]
    public async Task<IActionResult> SubmitApproval(int flowId, [FromBody] SubmitApprovalRequest request)
    {
        if (request.Action == ApprovalAction.Pending)
            return BadRequest(new { code = 400, message = "请指定审批动作" });
        if (string.IsNullOrWhiteSpace(request.Comment))
            return BadRequest(new { code = 400, message = "请填写审批意见" });

        var result = await _wfService.SubmitApprovalAsync(flowId, 0, request.Action, request.Comment);
        if (result == null) return NotFound(new { code = 404, message = "流程不存在" });
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var flow = await _wfService.GetFlowByIdAsync(id);
        if (flow == null) return NotFound(new { code = 404, message = "流程不存在" });

        var wfNodes = _wfService.GetStandardWorkflow(flow.FlowType);
        var currentNode = wfNodes?.FirstOrDefault(n => n.NodeIndex == flow.CurrentNodeId);

        var detail = new FlowDetailResponse
        {
            Id = flow.Id,
            ProjectId = flow.ProjectId,
            FlowType = flow.FlowType,
            FlowTypeName = GetFlowTypeName(flow.FlowType),
            FlowNo = flow.FlowNo,
            Title = flow.Title,
            Status = flow.Status,
            StatusName = GetStatusName(flow.Status),
            CurrentNodeId = flow.CurrentNodeId,
            Amount = flow.Amount,
            Description = flow.Description,
            InitiatedAt = flow.InitiatedAt,
            CompletedAt = flow.CompletedAt,
            Approvals = flow.Approvals?.Select(a => new ApprovalSummary
            {
                Id = a.Id,
                ApproverName = a.Approver?.RealName ?? "审批人",
                NodeIndex = a.NodeIndex,
                Action = a.Action,
                Comment = a.Comment,
                ApprovedAt = a.ApprovedAt
            }).ToList(),
            WorkflowNodes = wfNodes,
            CurrentNode = currentNode
        };
        return Ok(new { code = 200, data = detail });
    }

    [HttpGet("pending/{userId}")]
    public async Task<IActionResult> GetPending(int userId)
    {
        var result = await _wfService.GetPendingApprovalsAsync(userId);
        return Ok(new { code = 200, data = result.Select(MapToResponse) });
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _wfService.GetFlowsByProjectAsync(projectId);
        return Ok(new { code = 200, data = result.Select(MapToResponse) });
    }

    [HttpGet("project/{projectId}/type/{flowType}")]
    public async Task<IActionResult> GetByType(int projectId, FlowType flowType)
    {
        var result = await _wfService.GetFlowsByTypeAsync(projectId, flowType);
        return Ok(new { code = 200, data = result.Select(MapToResponse) });
    }

    [HttpGet("workflows")]
    public IActionResult GetWorkflows()
    {
        var wfs = _wfService.GetAllStandardWorkflows();
        return Ok(new { code = 200, data = wfs });
    }

    [HttpGet("workflows/{flowType}")]
    public IActionResult GetWorkflow(FlowType flowType)
    {
        var wf = _wfService.GetStandardWorkflow(flowType);
        if (wf == null) return NotFound(new { code = 404, message = "未找到该流程类型" });
        return Ok(new { code = 200, data = wf });
    }

    [HttpPost("{id}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var result = await _wfService.ArchiveFlowAsync(id);
        if (!result) return NotFound(new { code = 404, message = "流程不存在" });
        return Ok(new { code = 200, message = "归档成功" });
    }

    private FlowResponse MapToResponse(BusinessFlow f) => new()
    {
        Id = f.Id, ProjectId = f.ProjectId, FlowType = f.FlowType,
        FlowNo = f.FlowNo, Title = f.Title, Status = f.Status,
        CurrentNodeId = f.CurrentNodeId, Amount = f.Amount,
        Description = f.Description, InitiatedAt = f.InitiatedAt,
        CompletedAt = f.CompletedAt
    };

    private static string GetFlowTypeName(FlowType t) => t switch
    {
        FlowType.ContactOrder => "工联单",
        FlowType.PriceConfirmation => "认质认价",
        FlowType.Visa => "签证",
        FlowType.Claim => "索赔",
        FlowType.DesignChange => "设计变更",
        _ => t.ToString()
    };

    private static string GetStatusName(FlowStatus s) => s switch
    {
        FlowStatus.Draft => "草稿",
        FlowStatus.Pending => "待审批",
        FlowStatus.Approved => "已通过",
        FlowStatus.Rejected => "已驳回",
        FlowStatus.Archived => "已归档",
        _ => s.ToString()
    };
}