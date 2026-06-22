using Microsoft.AspNetCore.Mvc;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _docService;
    public DocumentController(IDocumentService docService) { _docService = docService; }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] UploadDocumentRequest request)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { code = 400, message = "请选择要上传的文件" });
        if (file.Length > 100 * 1024 * 1024)
            return BadRequest(new { code = 400, message = "文件大小不能超过100MB" });

        var result = await _docService.UploadDocumentAsync(file, request, 0);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _docService.GetDocumentsByProjectAsync(projectId);
        return Ok(new { code = 200, data = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _docService.GetDocumentByIdAsync(id);
        if (result == null) return NotFound(new { code = 404, message = "文档不存在" });
        return Ok(new { code = 200, data = result });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _docService.DeleteDocumentAsync(id, 0);
        if (!result) return NotFound(new { code = 404, message = "文档不存在" });
        return Ok(new { code = 200, message = "删除成功" });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] DocumentCategory? category)
    {
        var result = await _docService.SearchDocumentsAsync(keyword ?? string.Empty, category);
        return Ok(new { code = 200, data = result });
    }
}