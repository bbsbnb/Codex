namespace TXConstructionManagement.Services.Interfaces;

public interface ISecondaryOperationService
{
    Task<bool> SubmitPlanningAsync(int projectId, string content, string responsiblePerson, int submitterId);
    Task<bool> ReviewPlanningAsync(int planId, bool approved, string comment, int reviewerId);
    Task<bool> ArchivePlanningAsync(int planId, int operatorId);
    Task<Models.MonthlyPlan?> GetPlanningByIdAsync(int planId);
    Task<List<Models.MonthlyPlan>> GetPlanningByProjectAsync(int projectId);
}