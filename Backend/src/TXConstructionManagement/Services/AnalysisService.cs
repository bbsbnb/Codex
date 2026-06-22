using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;

namespace TXConstructionManagement.Services;

public class AnalysisService
{
    private readonly AppDbContext _context;
    public AnalysisService(AppDbContext context) { _context = context; }

    public async Task<MonthlyAnalysisResponse> GetMonthlyAnalysisAsync(int projectId, int year, int month)
    {
        var projects = await _context.Projects.ToListAsync();
        var flows = await _context.BusinessFlows
            .Where(f => f.InitiatedAt.Year == year && f.InitiatedAt.Month == month)
            .ToListAsync();
        if (projectId > 0) flows = flows.Where(f => f.ProjectId == projectId).ToList();

        var payments = await _context.PaymentRecords
            .Where(p => p.CreatedAt.Year == year && p.CreatedAt.Month == month)
            .ToListAsync();
        if (projectId > 0) payments = payments.Where(p => p.ProjectId == projectId).ToList();

        var warnings = await _context.WarningRecords
            .Where(w => !w.IsResolved).ToListAsync();
        if (projectId > 0) warnings = warnings.Where(w => w.ProjectId == projectId).ToList();

        var insp = await _context.InspectionRecords.Where(i => i.Status == InspectionStatus.Open || i.Status == InspectionStatus.InProgress).ToListAsync();
        if (projectId > 0) insp = insp.Where(i => i.ProjectId == projectId).ToList();

        var meetings = await _context.ReviewMeetings
            .Where(m => m.MeetingDate.Year == year && m.MeetingDate.Month == month).ToListAsync();
        if (projectId > 0) meetings = meetings.Where(m => m.ProjectId == projectId).ToList();

        // Flow type summaries
        var flowSummaries = flows.GroupBy(f => f.FlowType).Select(g => new FlowTypeSummary
        {
            TypeName = g.Key switch {
                FlowType.ContactOrder => "工联单", FlowType.PriceConfirmation => "认质认价",
                FlowType.Visa => "签证", FlowType.Claim => "索赔",
                FlowType.DesignChange => "设计变更", FlowType.MonthlyValuation => "月验工计价",
                FlowType.MaterialSettlement => "材料结算", FlowType.ConsumptionVerification => "消耗量核定",
                _ => g.Key.ToString()
            },
            Count = g.Count(),
            TotalAmount = g.Where(x => x.Amount.HasValue).Sum(x => x.Amount ?? 0)
        }).ToList();

        return new MonthlyAnalysisResponse
        {
            ProjectCount = projects.Count,
            ActiveProjectCount = projects.Count(p => p.Status == ProjectStatus.Active),
            MonthlyFlowCount = flows.Count,
            TotalSettledAmount = flows.Where(f => f.Status == FlowStatus.Archived && f.Amount.HasValue).Sum(f => f.Amount ?? 0),
            PendingApprovalCount = flows.Count(f => f.Status == FlowStatus.Draft || f.Status == FlowStatus.Pending),
            OpenWarningCount = warnings.Count,
            OpenInspectionCount = insp.Count,
            TotalPaymentCollected = payments.Where(p => p.Status == PaymentStatus.Collected).Sum(p => p.Amount),
            TotalPaymentPending = payments.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount),
            ReviewMeetingCount = meetings.Count,
            FlowTypeSummaries = flowSummaries
        };
    }
}