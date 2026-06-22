using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Services;

public class WarningService : IWarningService
{
    private readonly AppDbContext _context;
    public WarningService(AppDbContext context) { _context = context; }

    public async Task CheckAndGenerateWarningsAsync()
    {
        // 1. 索赔28天时效检查
        var overdueClaims = await _context.BusinessFlows
            .Where(b => b.FlowType == FlowType.Claim && b.Status == FlowStatus.Draft
                && b.InitiatedAt.AddDays(28) < DateTime.Now)
            .ToListAsync();
        foreach (var claim in overdueClaims)
        {
            _context.WarningRecords.Add(new WarningRecord
            {
                ProjectId = claim.ProjectId, Type = WarningType.ClaimDeadline,
                Level = WarningLevel.Critical,
                Message = $"索赔单 {claim.FlowNo} 已超过28天时效，请立即处理"
            });
        }

        // 2. 超概5%预警
        var activeProjects = await _context.Projects.Where(p => p.Status == ProjectStatus.Active).ToListAsync();
        foreach (var project in activeProjects)
        {
            var totalSettlements = await _context.BusinessFlows
                .Where(b => b.ProjectId == project.Id && b.Status == FlowStatus.Archived
                    && (b.FlowType == FlowType.MaterialSettlement || b.FlowType == FlowType.MonthlyValuation))
                .SumAsync(b => b.Amount ?? 0);

            if (project.ContractAmount > 0 && totalSettlements > project.ContractAmount * 1.05m)
            {
                _context.WarningRecords.Add(new WarningRecord
                {
                    ProjectId = project.Id, Type = WarningType.OverBudget,
                    Level = WarningLevel.Critical,
                    Message = $"项目 {project.ProjectName} 成本已超概算5%，请于3日内提交书面说明"
                });
            }
        }

        // 3. 材料偏差预警（消耗量超计划10%）
        var consumptions = await _context.SettlementDetails
            .Where(d => d.SettlementType == SettlementType.ConsumptionVerification && d.DeviationRate > 10)
            .Include(d => d.Flow).ThenInclude(f => f!.Project)
            .ToListAsync();
        foreach (var c in consumptions)
        {
            _context.WarningRecords.Add(new WarningRecord
            {
                ProjectId = c.Flow?.ProjectId ?? 0,
                Type = WarningType.MaterialDeviation,
                Level = WarningLevel.Warning,
                Message = $"材料消耗偏差率{c.DeviationRate}%，实际消耗{c.ActualQuantity}超过计划{c.PlannedQuantity}"
            });
        }

        // 4. 计划逾期预警
        var overduePlans = await _context.MonthlyPlans
            .Where(p => p.Status == PlanStatus.Pending && p.DueDate < DateTime.Now).ToListAsync();
        foreach (var plan in overduePlans)
        {
            _context.WarningRecords.Add(new WarningRecord
            {
                ProjectId = plan.ProjectId, Type = WarningType.PlanOverdue,
                Level = WarningLevel.Warning,
                Message = $"月度计划 '{plan.PlanName}' 已逾期，负责人: {plan.ResponsiblePerson}"
            });
        }

        // 5. 检查整改逾期预警
        var overdueInsp = await _context.InspectionRecords
            .Where(i => i.Status == InspectionStatus.Open && i.RectificationDeadline < DateTime.Now).ToListAsync();
        foreach (var ins in overdueInsp)
        {
            _context.WarningRecords.Add(new WarningRecord
            {
                ProjectId = ins.ProjectId, Type = WarningType.PlanOverdue,
                Level = WarningLevel.Warning,
                Message = $"检查问题已过整改期限: {ins.IssueDescription}"
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<WarningRecord>> GetUnresolvedWarningsAsync(int projectId)
    {
        return await _context.WarningRecords
            .Where(w => w.ProjectId == projectId && !w.IsResolved)
            .OrderByDescending(w => w.TriggeredAt).ToListAsync();
    }

    public async Task ResolveWarningAsync(int warningId, int operatorId)
    {
        var warning = await _context.WarningRecords.FindAsync(warningId);
        if (warning == null) return;
        warning.IsResolved = true;
        warning.ResolvedAt = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task<List<WarningRecord>> GetWarningsByTypeAsync(WarningType type)
    {
        return await _context.WarningRecords.Where(w => w.Type == type)
            .OrderByDescending(w => w.TriggeredAt).ToListAsync();
    }

    public async Task CreateWarningAsync(int projectId, WarningType type, WarningLevel level, string message)
    {
        _context.WarningRecords.Add(new WarningRecord
        {
            ProjectId = projectId, Type = type, Level = level, Message = message
        });
        await _context.SaveChangesAsync();
    }
}