using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.Services;

namespace TXConstructionManagement.Controllers;

[ApiController, Route("api/analysis")]
public class AnalysisController : ControllerBase
{
    private readonly AnalysisService _svc;
    public AnalysisController(AnalysisService svc) { _svc = svc; }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly([FromQuery] int projectId = 0, [FromQuery] int year = 0, [FromQuery] int month = 0)
    {
        if (year == 0) year = DateTime.Now.Year;
        if (month == 0) month = DateTime.Now.Month;
        var r = await _svc.GetMonthlyAnalysisAsync(projectId, year, month);
        return Ok(new { code = 200, data = r });
    }
}