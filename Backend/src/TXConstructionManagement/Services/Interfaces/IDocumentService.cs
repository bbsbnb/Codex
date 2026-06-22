using TXConstructionManagement.DTOs;

namespace TXConstructionManagement.Services.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponse> UploadDocumentAsync(IFormFile file, UploadDocumentRequest request, int uploaderId);
    Task<List<DocumentResponse>> GetDocumentsByProjectAsync(int projectId);
    Task<DocumentResponse?> GetDocumentByIdAsync(int documentId);
    Task<bool> DeleteDocumentAsync(int documentId, int operatorId);
    Task<string> GetDocumentUrlAsync(int documentId);
    Task<List<DocumentResponse>> SearchDocumentsAsync(string keyword, DocumentCategory? category = null);
}