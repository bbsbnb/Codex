using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Services;

public class DocumentService : IDocumentService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public DocumentService(AppDbContext context, IConfiguration config) { _context = context; _config = config; }

    public async Task<DocumentResponse> UploadDocumentAsync(IFormFile file, UploadDocumentRequest request, int uploaderId)
    {
        var baseDir = _config["FileStorage:BaseUrl"] ?? "C:/TXConstruction/Documents";
        var projectDir = Path.Combine(baseDir, "Project_" + request.ProjectId, request.Category.ToString());
        Directory.CreateDirectory(projectDir);

        var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(projectDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var doc = new PrimaryDocument
        {
            ProjectId = request.ProjectId,
            Category = request.Category,
            FileName = file.FileName,
            FilePath = filePath,
            FileSize = file.Length,
            FileType = Path.GetExtension(file.FileName).ToLower(),
            UploadedBy = uploaderId,
            Remark = request.Remark
        };

        _context.PrimaryDocuments.Add(doc);
        await _context.SaveChangesAsync();
        return MapToResponse(doc);
    }

    public async Task<List<DocumentResponse>> GetDocumentsByProjectAsync(int projectId)
    {
        var docs = await _context.PrimaryDocuments.Where(d => d.ProjectId == projectId).OrderByDescending(d => d.UploadDate).ToListAsync();
        return docs.Select(MapToResponse).ToList();
    }

    public async Task<DocumentResponse?> GetDocumentByIdAsync(int documentId)
    {
        var doc = await _context.PrimaryDocuments.FindAsync(documentId);
        return doc == null ? null : MapToResponse(doc);
    }

    public async Task<bool> DeleteDocumentAsync(int documentId, int operatorId)
    {
        var doc = await _context.PrimaryDocuments.FindAsync(documentId);
        if (doc == null) return false;
        if (File.Exists(doc.FilePath)) File.Delete(doc.FilePath);
        _context.PrimaryDocuments.Remove(doc);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GetDocumentUrlAsync(int documentId)
    {
        var doc = await _context.PrimaryDocuments.FindAsync(documentId);
        return doc?.FilePath ?? string.Empty;
    }

    public async Task<List<DocumentResponse>> SearchDocumentsAsync(string keyword, DocumentCategory? category = null)
    {
        var query = _context.PrimaryDocuments.AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(d => d.FileName.Contains(keyword) || d.Remark.Contains(keyword));
        if (category.HasValue)
            query = query.Where(d => d.Category == category.Value);
        var docs = await query.OrderByDescending(d => d.UploadDate).ToListAsync();
        return docs.Select(MapToResponse).ToList();
    }

    private DocumentResponse MapToResponse(PrimaryDocument d) => new()
    {
        Id = d.Id, ProjectId = d.ProjectId, Category = d.Category,
        FileName = d.FileName, FileSize = d.FileSize, FileType = d.FileType,
        UploadDate = d.UploadDate, Remark = d.Remark
    };
}