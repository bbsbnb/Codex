namespace TXConstructionManagement.Models;

public class SystemNotification
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationLevel Level { get; set; } = NotificationLevel.Info;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ReadAt { get; set; }
}

public enum NotificationType
{
    System = 0,
    Approval = 1,
    Warning = 2,
    Meeting = 3,
    Plan = 4
}

public enum NotificationLevel
{
    Info = 0,
    Important = 1,
    Urgent = 2
}