using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/settlement")]
public class SettlementController : ControllerBase
{
    private readonly SettlementService _svc;

    public SettlementController(SettlementService svc) { _svc = svc; }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateSettlementRequest request)
    {
        if (request.ProjectId <= 0) return BadRequest(new { code = 400, message = "请选择项目" });
        if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest(new { code = 400, message = "标题不能为空" });
        var result = await _svc.CreateSettlementAsync(request, 0);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _svc.GetSettlementsByProjectAsync(projectId);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _svc.GetByIdAsync(id);
        if (result == null) return NotFound(new { code = 404, message = "结算单不存在" });
        return Ok(new { code = 200, data = result });
    }

    // 建造合同三张表
    [HttpPost("contract-table/generate")]
    public async Task<IActionResult> GenerateContractTable([FromQuery] int projectId, [FromQuery] int year, [FromQuery] int month)
    {
        try
        {
            var result = await _svc.GenerateContractTableAsync(projectId, year, month);
            return Ok(new { code = 200, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { code = 400, message = ex.Message });
        }
    }

    [HttpGet("contract-table")]
    public async Task<IActionResult> GetContractTable([FromQuery] int projectId, [FromQuery] int year, [FromQuery] int month)
    {
        var result = await _svc.GetContractTableAsync(projectId, year, month);
        if (result == null) return NotFound(new { code = 404, message = "暂未生成建造合同表，请先点击生成" });
        return Ok(new { code = 200, data = result });
    }
}