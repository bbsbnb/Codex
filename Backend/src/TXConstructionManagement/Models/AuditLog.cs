namespace TXConstructionManagement.Models;

public class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string IpAddress { get; set; } = string.Empty;
}