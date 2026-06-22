namespace TXConstructionManagement.Models;

public class PrimaryDocument
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public DocumentCategory Category { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public int UploadedBy { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.Now;
    public string Remark { get; set; } = string.Empty;
    public Project? Project { get; set; }
}

public enum DocumentCategory
{
    ConstructionContract = 0,
    TenderDocument = 1,
    BuildingContract = 2,
    BusinessPlan = 3,
    PrimaryDisclosure = 4,
    ConstructionOrganization = 5
}