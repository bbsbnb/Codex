namespace TXConstructionManagement.Services.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(string userId, string actionType, string entityName, int? entityId, string details, string ipAddress);
    Task<List<dynamic>> GetLogsByUserAsync(int userId, int pageSize = 50, int pageNumber = 1);
    Task<List<dynamic>> GetLogsByEntityAsync(string entityName, int? entityId, int pageSize = 50, int pageNumber = 1);
    Task<List<dynamic>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageSize = 100, int pageNumber = 1);
}