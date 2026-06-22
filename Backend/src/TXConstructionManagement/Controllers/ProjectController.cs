using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectController(IProjectService projectService) { _projectService = projectService; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProjectCode) || string.IsNullOrWhiteSpace(request.ProjectName))
            return BadRequest(new { code = 400, message = "项目编号和名称不能为空" });
        var result = await _projectService.CreateProjectAsync(request, 0);
        return Ok(new { code = 200, data = result });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectRequest request)
    {
        var result = await _projectService.UpdateProjectAsync(id, request, 0);
        if (!result) return NotFound(new { code = 404, message = "项目不存在" });
        return Ok(new { code = 200, message = "更新成功" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _projectService.DeleteProjectAsync(id, 0);
        if (!result) return NotFound(new { code = 404, message = "项目不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _projectService.GetProjectByIdAsync(id);
        if (result == null) return NotFound(new { code = 404, message = "项目不存在" });
        return Ok(new { code = 200, data = result });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _projectService.GetAllProjectsAsync();
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(ProjectStatus status)
    {
        var result = await _projectService.GetProjectsByStatusAsync(status);
        return Ok(new { code = 200, data = result });
    }

    [HttpPost("{projectId}/member/{userId}")]
    public async Task<IActionResult> AssignMember(int projectId, int userId, [FromQuery] Position position)
    {
        var result = await _projectService.AssignMemberAsync(projectId, userId, position, 0);
        if (!result) return Conflict(new { code = 409, message = "该成员已在项目中" });
        return Ok(new { code = 200, message = "分配成功" });
    }

    [HttpDelete("{projectId}/member/{userId}")]
    public async Task<IActionResult> RemoveMember(int projectId, int userId)
    {
        var result = await _projectService.RemoveMemberAsync(projectId, userId, 0);
        if (!result) return NotFound(new { code = 404, message = "未找到该成员" });
        return Ok(new { code = 200, message = "移除成功" });
    }
}