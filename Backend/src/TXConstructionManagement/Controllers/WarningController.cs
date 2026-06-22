using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Controllers;

[ApiController, Route("api/warning")]
public class WarningController : ControllerBase
{
    private readonly IWarningService _warnService;

    public WarningController(IWarningService warnService) { _warnService = warnService; }

    [HttpGet("check")]
    public async Task<IActionResult> CheckAndGenerate()
    {
        await _warnService.CheckAndGenerateWarningsAsync();
        return Ok(new { code = 200, message = "预警检查完成" });
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetUnresolved(int projectId)
    {
        var result = await _warnService.GetUnresolvedWarningsAsync(projectId);
        return Ok(new { code = 200, data = result });
    }

    [HttpPost("{id}/resolve")]
    public async Task<IActionResult> Resolve(int id)
    {
        await _warnService.ResolveWarningAsync(id, 0);
        return Ok(new { code = 200, message = "已处理" });
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType(WarningType type)
    {
        var result = await _warnService.GetWarningsByTypeAsync(type);
        return Ok(new { code = 200, data = result });
    }

    [HttpPost("batch-resolve/{projectId}")]
    public async Task<IActionResult> BatchResolve(int projectId)
    {
        var warnings = await _warnService.GetUnresolvedWarningsAsync(projectId);
        foreach (var w in warnings) await _warnService.ResolveWarningAsync(w.Id, 0);
        return Ok(new { code = 200, message = "已处理" + warnings.Count + "条预警" });
    }
}