using TXConstructionManagement.Data;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;
    public AuditLogService(AppDbContext context) { _context = context; }

    public async Task LogAsync(string userIdStr, string actionType, string entityName, int? entityId, string details, string ipAddress)
    {
        int.TryParse(userIdStr, out var userId);
        _context.AuditLogs.Add(new AuditLog
        {
            UserId = userId, ActionType = actionType, EntityName = entityName,
            EntityId = entityId, Details = details, IpAddress = ipAddress
        });
        await _context.SaveChangesAsync();
    }

    public async Task<List<dynamic>> GetLogsByUserAsync(int userId, int pageSize = 50, int pageNumber = 1)
    {
        return (await _context.AuditLogs.Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync())
            .Select(l => (dynamic)l).ToList();
    }

    public async Task<List<dynamic>> GetLogsByEntityAsync(string entityName, int? entityId, int pageSize = 50, int pageNumber = 1)
    {
        var query = _context.AuditLogs.Where(l => l.EntityName == entityName);
        if (entityId.HasValue) query = query.Where(l => l.EntityId == entityId);
        return (await query.OrderByDescending(l => l.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync())
            .Select(l => (dynamic)l).ToList();
    }

    public async Task<List<dynamic>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageSize = 100, int pageNumber = 1)
    {
        return (await _context.AuditLogs.Where(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate)
            .OrderByDescending(l => l.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync())
            .Select(l => (dynamic)l).ToList();
    }
}