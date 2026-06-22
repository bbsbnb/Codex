namespace TXConstructionManagement.Services.Interfaces;

public interface IWarningService
{
    Task CheckAndGenerateWarningsAsync();
    Task<List<WarningRecord>> GetUnresolvedWarningsAsync(int projectId);
    Task ResolveWarningAsync(int warningId, int operatorId);
    Task<List<WarningRecord>> GetWarningsByTypeAsync(WarningType type);
    Task CreateWarningAsync(int projectId, WarningType type, WarningLevel level, string message);
}