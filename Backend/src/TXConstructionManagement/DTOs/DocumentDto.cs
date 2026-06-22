namespace TXConstructionManagement.DTOs;

public class UploadDocumentRequest
{
    public int ProjectId { get; set; }
    public DocumentCategory Category { get; set; }
    public string Remark { get; set; } = string.Empty;
}

public class DocumentResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public DocumentCategory Category { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string Remark { get; set; } = string.Empty;
}