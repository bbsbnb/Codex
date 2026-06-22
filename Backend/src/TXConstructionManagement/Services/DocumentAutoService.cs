using TXConstructionManagement.Data;
using TXConstructionManagement.Models;

namespace TXConstructionManagement.Services;

public class DocumentAutoService
{
    private readonly AppDbContext _context;
    public DocumentAutoService(AppDbContext context) { _context = context; }

    public string AutoClassify(string fileName, string? remark)
    {
        var lower = fileName.ToLower();
        if (remark != null) lower += " " + remark.ToLower();

        if (lower.Contains("合同")) return "施工合同";
        if (lower.Contains("投标") || lower.Contains("招标")) return "招投标文件";
        if (lower.Contains("建造合同") || lower.Contains("目标")) return "建造合同";
        if (lower.Contains("策划") || lower.Contains("商务")) return "商务策划";
        if (lower.Contains("交底")) return "一次经营交底";
        if (lower.Contains("施工组织") || lower.Contains("施工方案") || lower.Contains("施组")) return "施工组织设计";

        return "其他";
    }

    public async Task<List<PrimaryDocument>> GetSharedDocumentsAsync(int projectId, DocumentCategory? category = null)
    {
        var query = _context.PrimaryDocuments.Where(d => d.ProjectId == projectId);
        if (category.HasValue) query = query.Where(d => d.Category == category.Value);
        return await Task.FromResult(query.ToList());
    }

    public async Task<Dictionary<string, int>> GetDocumentStatsAsync(int projectId)
    {
        var docs = _context.PrimaryDocuments.Where(d => d.ProjectId == projectId).ToList();
        var stats = docs.GroupBy(d => d.Category)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());
        return await Task.FromResult(stats);
    }
}