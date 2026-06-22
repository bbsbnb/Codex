using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyPlanController : ControllerBase
{
    private readonly IMonthlyPlanService _planService;
    public MonthlyPlanController(IMonthlyPlanService planService) { _planService = planService; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanRequest request)
    {
        if (request.ProjectId <= 0) return BadRequest(new { code = 400, message = "请选择项目" });
        if (string.IsNullOrWhiteSpace(request.PlanName)) return BadRequest(new { code = 400, message = "计划名称不能为空" });
        var result = await _planService.CreatePlanAsync(request, 0);
        return Ok(new { code = 200, data = result });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePlanRequest request)
    {
        var result = await _planService.UpdatePlanAsync(id, request, 0);
        if (!result) return NotFound(new { code = 404, message = "计划不存在" });
        return Ok(new { code = 200, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _planService.DeletePlanAsync(id, 0);
        if (!result) return NotFound(new { code = 404, message = "计划不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _planService.GetPlanByIdAsync(id);
        if (result == null) return NotFound(new { code = 404, message = "计划不存在" });
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _planService.GetPlansByProjectAsync(projectId);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("period/{projectId}/{year}/{month}")]
    public async Task<IActionResult> GetByPeriod(int projectId, int year, int month)
    {
        var result = await _planService.GetPlansByPeriodAsync(projectId, year, month);
        return Ok(new { code = 200, data = result });
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var result = await _planService.CompletePlanAsync(id, 0);
        if (!result) return NotFound(new { code = 404, message = "计划不存在" });
        return Ok(new { code = 200, message = "已完成" });
    }
}