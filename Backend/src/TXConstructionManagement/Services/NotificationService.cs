using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;

namespace TXConstructionManagement.Services;

public class NotificationService
{
    private readonly AppDbContext _context;
    public NotificationService(AppDbContext context) { _context = context; }

    public async Task<NotificationResponse> CreateAsync(CreateNotificationRequest req)
    {
        var n = new SystemNotification
        {
            UserId = req.UserId, ProjectId = req.ProjectId,
            Title = req.Title, Content = req.Content,
            Type = req.Type, Level = req.Level
        };
        _context.SystemNotifications.Add(n);
        await _context.SaveChangesAsync();
        return MapToResponse(n);
    }

    public async Task<List<NotificationResponse>> GetByUserAsync(int userId)
    {
        var list = await _context.SystemNotifications
            .Where(n => n.UserId == null || n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt).Take(50).ToListAsync();
        return list.Select(MapToResponse).ToList();
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var n = await _context.SystemNotifications.FindAsync(id);
        if (n == null) return false;
        n.IsRead = true; n.ReadAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.SystemNotifications
            .Where(n => (n.UserId == null || n.UserId == userId) && !n.IsRead).CountAsync();
    }

    public async Task MarkAllReadAsync(int userId)
    {
        var unread = await _context.SystemNotifications
            .Where(n => (n.UserId == null || n.UserId == userId) && !n.IsRead).ToListAsync();
        unread.ForEach(n => { n.IsRead = true; n.ReadAt = DateTime.Now; });
        await _context.SaveChangesAsync();
    }

    private NotificationResponse MapToResponse(SystemNotification n) => new()
    {
        Id = n.Id, Title = n.Title, Content = n.Content,
        TypeName = n.Type switch { NotificationType.System => "系统", NotificationType.Approval => "审批", NotificationType.Warning => "预警", NotificationType.Meeting => "会议", NotificationType.Plan => "计划", _ => "" },
        LevelName = n.Level switch { NotificationLevel.Info => "普通", NotificationLevel.Important => "重要", NotificationLevel.Urgent => "紧急", _ => "" },
        IsRead = n.IsRead, CreatedAt = n.CreatedAt
    };
}