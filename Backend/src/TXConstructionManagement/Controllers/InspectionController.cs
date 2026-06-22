using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/inspection")]
public class InspectionController : ControllerBase
{
    private readonly InspectionService _svc;
    public InspectionController(InspectionService svc) { _svc = svc; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInspectionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IssueDescription))
            return BadRequest(new { code = 400, message = "请描述问题" });
        var result = await _svc.CreateAsync(request, 0);
        return Ok(new { code = 200, data = result });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInspectionRequest request)
    {
        var result = await _svc.UpdateAsync(id, request);
        if (!result) return NotFound(new { code = 404, message = "记录不存在" });
        return Ok(new { code = 200, message = "更新成功" });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _svc.GetByIdAsync(id);
        if (result == null) return NotFound(new { code = 404, message = "记录不存在" });
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId, [FromQuery] InspectionType? type)
    {
        var result = await _svc.GetByProjectAsync(projectId, type);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType(InspectionType type)
    {
        var result = await _svc.GetByTypeAsync(type);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("summary/{projectId}")]
    public async Task<IActionResult> GetSummary(int projectId)
    {
        var result = await _svc.GetSummaryAsync(projectId);
        return Ok(new { code = 200, data = result });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _svc.DeleteAsync(id);
        if (!result) return NotFound(new { code = 404, message = "记录不存在" });
        return Ok(new { code = 200, message = "已删除" });
    }
}