using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Services;

namespace TXConstructionManagement.Controllers;

[ApiController, Route("api/review-meeting")]
public class ReviewMeetingController : ControllerBase
{
    private readonly ReviewMeetingService _svc;
    public ReviewMeetingController(ReviewMeetingService svc) { _svc = svc; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewMeetingRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.MeetingTitle)) return BadRequest(new { code = 400, message = "请输入会议标题" });
        var r = await _svc.CreateAsync(req, 0);
        return Ok(new { code = 200, data = r });
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(int id) { var r = await _svc.CompleteAsync(id); if (!r) return NotFound(); return Ok(new { code = 200 }); }

    [HttpPost("{id}/archive")]
    public async Task<IActionResult> Archive(int id) { var r = await _svc.ArchiveAsync(id); if (!r) return NotFound(); return Ok(new { code = 200 }); }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId) { return Ok(new { code = 200, data = await _svc.GetByProjectAsync(projectId) }); }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) { var r = await _svc.GetByIdAsync(id); if (r == null) return NotFound(); return Ok(new { code = 200, data = r }); }
}