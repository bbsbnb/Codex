using TXConstructionManagement.Models;

namespace TXConstructionManagement.DTOs;

public class CreatePaymentRequest
{
    public int ProjectId { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int ResponsibleUserId { get; set; }
    public string Remark { get; set; } = string.Empty;
}

public class UpdatePaymentRequest
{
    public PaymentStatus? Status { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Remark { get; set; }
}

public class PaymentResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string PaymentNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Remark { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}