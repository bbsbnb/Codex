using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Services;

public class SecondaryOperationService : ISecondaryOperationService
{
    private readonly AppDbContext _context;
    public SecondaryOperationService(AppDbContext context) { _context = context; }

    public async Task<bool> SubmitPlanningAsync(int projectId, string content, string responsiblePerson, int submitterId)
    {
        var year = DateTime.Now.Year; var month = DateTime.Now.Month;
        var existing = await _context.MonthlyPlans
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.Year == year && p.Month == month && p.PlanType == PlanType.SecondaryOperation);
        if (existing != null) return false;

        _context.MonthlyPlans.Add(new MonthlyPlan
        {
            ProjectId = projectId, Year = year, Month = month,
            PlanName = $"二次经营策划书-{year}年{month}月", PlanType = PlanType.SecondaryOperation,
            Content = content, ResponsiblePerson = responsiblePerson,
            DueDate = DateTime.Now.AddDays(7)
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReviewPlanningAsync(int planId, bool approved, string comment, int reviewerId)
    {
        var plan = await _context.MonthlyPlans.FindAsync(planId);
        if (plan == null) return false;
        plan.Status = approved ? PlanStatus.Completed : PlanStatus.Pending;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchivePlanningAsync(int planId, int operatorId)
    {
        var plan = await _context.MonthlyPlans.FindAsync(planId);
        if (plan == null) return false;
        plan.Status = PlanStatus.Completed;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<MonthlyPlan?> GetPlanningByIdAsync(int planId)
    {
        return await _context.MonthlyPlans.FindAsync(planId);
    }

    public async Task<List<MonthlyPlan>> GetPlanningByProjectAsync(int projectId)
    {
        return await _context.MonthlyPlans
            .Where(p => p.ProjectId == projectId && p.PlanType == PlanType.SecondaryOperation)
            .OrderByDescending(p => p.CreatedAt).ToListAsync();
    }
}