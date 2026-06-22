using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) { _authService = authService; }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { code = 400, message = "用户名和密码不能为空" });
        var result = await _authService.LoginAsync(request);
        if (result == null) return Unauthorized(new { code = 401, message = "用户名或密码错误" });
        return Ok(new { code = 200, data = result });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { code = 400, message = "用户名和密码不能为空" });
        var result = await _authService.RegisterAsync(request);
        if (!result) return Conflict(new { code = 409, message = "用户名已存在" });
        return Ok(new { code = 200, message = "注册成功" });
    }
}