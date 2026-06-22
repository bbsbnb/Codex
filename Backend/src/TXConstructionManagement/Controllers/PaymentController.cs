using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Services;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _svc;
    public PaymentController(PaymentService svc) { _svc = svc; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
    {
        if (request.Amount <= 0) return BadRequest(new { code = 400, message = "金额必须大于0" });
        if (request.ProjectId <= 0) return BadRequest(new { code = 400, message = "请选择项目" });
        var result = await _svc.CreateAsync(request);
        return Ok(new { code = 200, data = result });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentRequest request)
    {
        var result = await _svc.UpdateAsync(id, request);
        if (!result) return NotFound(new { code = 404, message = "记录不存在" });
        return Ok(new { code = 200, message = "更新成功" });
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _svc.GetByProjectAsync(projectId);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _svc.GetByIdAsync(id);
        if (result == null) return NotFound(new { code = 404, message = "记录不存在" });
        return Ok(new { code = 200, data = result });
    }
}