using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/workflow-statistics")]
public class WorkflowStatisticsController : ControllerBase
{
    private readonly IWorkflowEngineService _wfService;

    public WorkflowStatisticsController(IWorkflowEngineService wfService) { _wfService = wfService; }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetProjectStatistics(int projectId)
    {
        var flows = await _wfService.GetFlowsByProjectAsync(projectId);
        var groups = flows.GroupBy(f => f.FlowType).Select(g => new
        {
            FlowType = (int)g.Key,
            FlowTypeName = g.Key switch
            {
                Models.FlowType.ContactOrder => "工联单",
                Models.FlowType.PriceConfirmation => "认质认价",
                Models.FlowType.Visa => "签证",
                Models.FlowType.Claim => "索赔",
                Models.FlowType.DesignChange => "设计变更",
                _ => g.Key.ToString()
            },
            Total = g.Count(),
            Draft = g.Count(f => f.Status == Models.FlowStatus.Draft),
            Pending = g.Count(f => f.Status == Models.FlowStatus.Pending),
            Approved = g.Count(f => f.Status == Models.FlowStatus.Approved),
            Archived = g.Count(f => f.Status == Models.FlowStatus.Archived),
            Rejected = g.Count(f => f.Status == Models.FlowStatus.Rejected),
            TotalAmount = g.Where(f => f.Amount.HasValue).Sum(f => f.Amount.Value)
        }).ToList();

        return Ok(new { code = 200, data = new { total = flows.Count, groups } });
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var projects = await _wfService.GetFlowsByProjectAsync(0);
        // Simplified - return empty group
        return Ok(new { code = 200, data = new { total = 0 } });
    }
}