using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Services;

namespace TXConstructionManagement.Controllers;

[ApiController, Route("api/notification")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _svc;
    public NotificationController(NotificationService svc) { _svc = svc; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationRequest req) { return Ok(new { code = 200, data = await _svc.CreateAsync(req) }); }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId) { return Ok(new { code = 200, data = await _svc.GetByUserAsync(userId) }); }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(int id) { var r = await _svc.MarkAsReadAsync(id); if (!r) return NotFound(); return Ok(new { code = 200 }); }

    [HttpGet("unread/{userId}")]
    public async Task<IActionResult> UnreadCount(int userId) { return Ok(new { code = 200, data = await _svc.GetUnreadCountAsync(userId) }); }

    [HttpPost("read-all/{userId}")]
    public async Task<IActionResult> MarkAllRead(int userId) { await _svc.MarkAllReadAsync(userId); return Ok(new { code = 200 }); }
}