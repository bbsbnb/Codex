namespace TXConstructionManagement.Models;

public class PaymentRecord
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string PaymentNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public string Remark { get; set; } = string.Empty;
    public int ResponsibleUserId { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Project? Project { get; set; }
}

public enum PaymentStatus
{
    Pending = 0,
    Collected = 1,
    Partial = 2,
    Overdue = 3
}