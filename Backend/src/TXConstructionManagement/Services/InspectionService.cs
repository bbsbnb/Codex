using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;

namespace TXConstructionManagement.Services;

public class InspectionService
{
    private readonly AppDbContext _context;

    public InspectionService(AppDbContext context) { _context = context; }

    public async Task<InspectionResponse> CreateAsync(CreateInspectionRequest request, int inspectorId)
    {
        var record = new InspectionRecord
        {
            ProjectId = request.ProjectId, Type = request.Type,
            IssueDescription = request.IssueDescription, IssueCategory = request.IssueCategory,
            InspectorId = inspectorId, InspectionDate = DateTime.Now,
            RectificationRequirement = request.RectificationRequirement,
            RectificationDeadline = request.RectificationDeadline,
            Status = InspectionStatus.Open
        };
        _context.InspectionRecords.Add(record);
        await _context.SaveChangesAsync();
        return MapToResponse(record);
    }

    public async Task<bool> UpdateAsync(int id, UpdateInspectionRequest request)
    {
        var record = await _context.InspectionRecords.FindAsync(id);
        if (record == null) return false;
        if (request.Result != null) record.Result = request.Result;
        if (request.Status.HasValue) record.Status = request.Status.Value;
        if (request.RectificationRequirement != null) record.RectificationRequirement = request.RectificationRequirement;
        if (request.RectificationDeadline.HasValue) record.RectificationDeadline = request.RectificationDeadline;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<InspectionResponse>> GetByProjectAsync(int projectId, InspectionType? type = null)
    {
        var query = _context.InspectionRecords.Where(i => i.ProjectId == projectId);
        if (type.HasValue) query = query.Where(i => i.Type == type.Value);
        var records = await query.OrderByDescending(i => i.InspectionDate).ToListAsync();
        return records.Select(MapToResponse).ToList();
    }

    public async Task<List<InspectionResponse>> GetByTypeAsync(InspectionType type)
    {
        var records = await _context.InspectionRecords.Where(i => i.Type == type)
            .OrderByDescending(i => i.InspectionDate).ToListAsync();
        return records.Select(MapToResponse).ToList();
    }

    public async Task<InspectionResponse?> GetByIdAsync(int id)
    {
        var record = await _context.InspectionRecords.FindAsync(id);
        return record == null ? null : MapToResponse(record);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _context.InspectionRecords.FindAsync(id);
        if (record == null) return false;
        _context.InspectionRecords.Remove(record);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<InspectionResponse>> GetSummaryAsync(int projectId)
    {
        var all = await _context.InspectionRecords.Where(i => i.ProjectId == projectId).ToListAsync();
        return all.GroupBy(i => i.Type).Select(g =>
        {
            var first = g.First();
            return new InspectionResponse
            {
                Id = 0, ProjectId = projectId, Type = g.Key,
                TypeName = GetTypeName(g.Key),
                IssueDescription = $"共{g.Count()}条问题，未处理{g.Count(i => i.Status == InspectionStatus.Open)}条",
                IssueCategory = "",
                InspectionDate = DateTime.Now,
                RectificationRequirement = "",
                Status = InspectionStatus.Open,
                StatusName = "",
                Result = ""
            };
        }).ToList();
    }

    private InspectionResponse MapToResponse(InspectionRecord r) => new()
    {
        Id = r.Id, ProjectId = r.ProjectId, Type = r.Type,
        TypeName = GetTypeName(r.Type),
        IssueDescription = r.IssueDescription, IssueCategory = r.IssueCategory,
        InspectionDate = r.InspectionDate,
        RectificationRequirement = r.RectificationRequirement,
        RectificationDeadline = r.RectificationDeadline,
        Status = r.Status, StatusName = GetStatusName(r.Status), Result = r.Result
    };

    private static string GetTypeName(InspectionType t) => t switch
    {
        InspectionType.Quality => "质量管理",
        InspectionType.Safety => "安全管理",
        InspectionType.Progress => "进度管理",
        InspectionType.CivilConstruction => "现场文明施工",
        InspectionType.Contract => "合同管理",
        InspectionType.Material => "材料管理",
        InspectionType.Bidding => "对下招标管理",
        InspectionType.Document => "资料管理",
        _ => t.ToString()
    };

    private static string GetStatusName(InspectionStatus s) => s switch
    {
        InspectionStatus.Open => "待处理",
        InspectionStatus.InProgress => "整改中",
        InspectionStatus.Closed => "已闭环",
        InspectionStatus.Overdue => "已逾期",
        _ => s.ToString()
    };
}