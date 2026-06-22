using TXConstructionManagement.DTOs;

namespace TXConstructionManagement.Services.Interfaces;

public interface IMonthlyPlanService
{
    Task<PlanResponse> CreatePlanAsync(CreatePlanRequest request, int operatorId);
    Task<bool> UpdatePlanAsync(int planId, CreatePlanRequest request, int operatorId);
    Task<bool> DeletePlanAsync(int planId, int operatorId);
    Task<PlanResponse?> GetPlanByIdAsync(int planId);
    Task<List<PlanResponse>> GetPlansByProjectAsync(int projectId);
    Task<List<PlanResponse>> GetPlansByPeriodAsync(int projectId, int year, int month);
    Task<bool> CompletePlanAsync(int planId, int operatorId);
}