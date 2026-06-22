using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Services;

public class MonthlyPlanService : IMonthlyPlanService
{
    private readonly AppDbContext _context;
    public MonthlyPlanService(AppDbContext context) { _context = context; }

    public async Task<PlanResponse> CreatePlanAsync(CreatePlanRequest request, int operatorId)
    {
        var plan = new MonthlyPlan
        {
            ProjectId = request.ProjectId, Year = request.Year, Month = request.Month,
            PlanName = request.PlanName, PlanType = request.PlanType, Content = request.Content,
            ResponsiblePerson = request.ResponsiblePerson, DueDate = request.DueDate
        };
        _context.MonthlyPlans.Add(plan);
        await _context.SaveChangesAsync();
        return MapToResponse(plan);
    }

    public async Task<bool> UpdatePlanAsync(int planId, CreatePlanRequest request, int operatorId)
    {
        var plan = await _context.MonthlyPlans.FindAsync(planId);
        if (plan == null) return false;
        plan.PlanName = request.PlanName; plan.PlanType = request.PlanType;
        plan.Content = request.Content; plan.ResponsiblePerson = request.ResponsiblePerson;
        plan.DueDate = request.DueDate;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePlanAsync(int planId, int operatorId)
    {
        var plan = await _context.MonthlyPlans.FindAsync(planId);
        if (plan == null) return false;
        _context.MonthlyPlans.Remove(plan);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PlanResponse?> GetPlanByIdAsync(int planId)
    {
        var plan = await _context.MonthlyPlans.FindAsync(planId);
        return plan == null ? null : MapToResponse(plan);
    }

    public async Task<List<PlanResponse>> GetPlansByProjectAsync(int projectId)
    {
        var plans = await _context.MonthlyPlans.Where(p => p.ProjectId == projectId).OrderByDescending(p => p.Year).ThenByDescending(p => p.Month).ToListAsync();
        return plans.Select(MapToResponse).ToList();
    }

    public async Task<List<PlanResponse>> GetPlansByPeriodAsync(int projectId, int year, int month)
    {
        var plans = await _context.MonthlyPlans.Where(p => p.ProjectId == projectId && p.Year == year && p.Month == month).ToListAsync();
        return plans.Select(MapToResponse).ToList();
    }

    public async Task<bool> CompletePlanAsync(int planId, int operatorId)
    {
        var plan = await _context.MonthlyPlans.FindAsync(planId);
        if (plan == null) return false;
        plan.Status = PlanStatus.Completed;
        await _context.SaveChangesAsync();
        return true;
    }

    private PlanResponse MapToResponse(MonthlyPlan p) => new()
    {
        Id = p.Id, ProjectId = p.ProjectId, Year = p.Year, Month = p.Month,
        PlanName = p.PlanName, PlanType = p.PlanType, Content = p.Content,
        ResponsiblePerson = p.ResponsiblePerson, DueDate = p.DueDate,
        Status = p.Status, CreatedAt = p.CreatedAt
    };
}