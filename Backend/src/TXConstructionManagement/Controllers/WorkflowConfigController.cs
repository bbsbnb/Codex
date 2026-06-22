using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Services;

namespace TXConstructionManagement.Controllers;

[ApiController, Route("api/workflow-config")]
public class WorkflowConfigController : ControllerBase
{
    private readonly WorkflowConfigService _svc;
    public WorkflowConfigController(WorkflowConfigService svc) { _svc = svc; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkflowTemplateRequest req) { return Ok(new { code = 200, data = await _svc.CreateAsync(req) }); }

    [HttpGet]
    public async Task<IActionResult> GetAll() { return Ok(new { code = 200, data = await _svc.GetAllAsync() }); }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) { var r = await _svc.DeleteAsync(id); if (!r) return NotFound(); return Ok(new { code = 200 }); }

    [HttpPost("{id}/active/{active}")]
    public async Task<IActionResult> SetActive(int id, bool active) { var r = await _svc.SetActiveAsync(id, active); if (!r) return NotFound(); return Ok(new { code = 200 }); }
}