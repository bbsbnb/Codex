using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;

namespace TXConstructionManagement.Services;

public class PaymentService
{
    private readonly AppDbContext _context;
    public PaymentService(AppDbContext context) { _context = context; }

    public async Task<PaymentResponse> CreateAsync(CreatePaymentRequest request)
    {
        var maxNo = await _context.PaymentRecords.CountAsync();
        var record = new PaymentRecord
        {
            ProjectId = request.ProjectId,
            PaymentNo = $"HK-{DateTime.Now:yyyyMMdd}-{maxNo + 1:D4}",
            Amount = request.Amount,
            PaymentDate = request.PaymentDate ?? DateTime.Now,
            Remark = request.Remark,
            ResponsibleUserId = request.ResponsibleUserId,
            Status = PaymentStatus.Pending
        };
        _context.PaymentRecords.Add(record);
        await _context.SaveChangesAsync();
        return await MapToResponse(record);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePaymentRequest request)
    {
        var record = await _context.PaymentRecords.FindAsync(id);
        if (record == null) return false;
        if (request.Status.HasValue) record.Status = request.Status.Value;
        if (request.Amount.HasValue) record.Amount = request.Amount.Value;
        if (request.PaymentDate.HasValue) record.PaymentDate = request.PaymentDate.Value;
        if (request.Remark != null) record.Remark = request.Remark;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<PaymentResponse>> GetByProjectAsync(int projectId)
    {
        var records = await _context.PaymentRecords.Where(p => p.ProjectId == projectId)
            .OrderByDescending(p => p.CreatedAt).ToListAsync();
        var responses = new List<PaymentResponse>();
        foreach (var r in records) responses.Add(await MapToResponse(r));
        return responses;
    }

    public async Task<PaymentResponse?> GetByIdAsync(int id)
    {
        var record = await _context.PaymentRecords.FindAsync(id);
        return record == null ? null : await MapToResponse(record);
    }

    private async Task<PaymentResponse> MapToResponse(PaymentRecord r)
    {
        var project = await _context.Projects.FindAsync(r.ProjectId);
        return new PaymentResponse
        {
            Id = r.Id, ProjectId = r.ProjectId,
            ProjectName = project?.ProjectName ?? "",
            PaymentNo = r.PaymentNo, Amount = r.Amount,
            PaymentDate = r.PaymentDate, Remark = r.Remark,
            ResponsiblePerson = r.ResponsibleUserId.ToString(),
            Status = r.Status,
            StatusName = r.Status switch
            {
                PaymentStatus.Pending => "待收款",
                PaymentStatus.Collected => "已到账",
                PaymentStatus.Partial => "部分到账",
                PaymentStatus.Overdue => "已逾期",
                _ => r.Status.ToString()
            },
            CreatedAt = r.CreatedAt
        };
    }
}