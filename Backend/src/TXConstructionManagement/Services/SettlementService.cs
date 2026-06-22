using Microsoft.EntityFrameworkCore;
using TXConstructionManagement.Data;
using TXConstructionManagement.DTOs;
using TXConstructionManagement.Models;
using TXConstructionManagement.Services.Interfaces;

namespace TXConstructionManagement.Services;

public class SettlementService
{
    private readonly AppDbContext _context;
    private readonly IWorkflowEngineService _wfService;

    public SettlementService(AppDbContext context, IWorkflowEngineService wfService)
    { _context = context; _wfService = wfService; }

    public async Task<SettlementResponse> CreateSettlementAsync(CreateSettlementRequest request, int userId)
    {
        FlowType flowType = request.SettlementType switch
        {
            SettlementType.MonthlyValuation => FlowType.MonthlyValuation,
            SettlementType.MaterialSettlement => FlowType.MaterialSettlement,
            SettlementType.ConsumptionVerification => FlowType.ConsumptionVerification,
            _ => FlowType.MonthlyValuation
        };

        var flow = new BusinessFlow
        {
            ProjectId = request.ProjectId,
            FlowType = flowType,
            Title = request.Title,
            Description = GetDescription(request),
            Amount = request.SubmittedAmount
        };
        flow = await _wfService.CreateFlowAsync(flow);

        var detail = new SettlementDetail
        {
            FlowId = flow.Id,
            SettlementType = request.SettlementType,
            SettlementMonth = request.SettlementMonth,
            CompletedQuantity = request.CompletedQuantity,
            QuantityUnit = request.QuantityUnit,
            SubmittedAmount = request.SubmittedAmount,
            MaterialName = request.MaterialName,
            MaterialSpec = request.MaterialSpec,
            SettlementQuantity = request.SettlementQuantity,
            UnitPrice = request.UnitPrice,
            TotalAmount = request.SettlementQuantity.HasValue && request.UnitPrice.HasValue
                ? request.SettlementQuantity.Value * request.UnitPrice.Value : null,
            Subcontractor = request.Subcontractor,
            PlannedQuantity = request.PlannedQuantity,
            ActualQuantity = request.ActualQuantity,
            DeviationRate = request.PlannedQuantity.HasValue && request.ActualQuantity.HasValue
                ? Math.Round((request.ActualQuantity.Value - request.PlannedQuantity.Value) / request.PlannedQuantity.Value * 100, 2)
                : null,
            DeviationReason = request.DeviationReason
        };
        _context.SettlementDetails.Add(detail);
        await _context.SaveChangesAsync();
        return MapToResponse(flow, detail);
    }

    public async Task<List<SettlementResponse>> GetSettlementsByProjectAsync(int projectId)
    {
        var flows = await _wfService.GetFlowsByProjectAsync(projectId);
        var settlementFlows = flows.Where(f => f.FlowType == FlowType.MonthlyValuation
            || f.FlowType == FlowType.MaterialSettlement
            || f.FlowType == FlowType.ConsumptionVerification).ToList();

        var flowIds = settlementFlows.Select(f => f.Id).ToList();
        var details = await _context.SettlementDetails.Where(d => flowIds.Contains(d.FlowId)).ToListAsync();

        return settlementFlows.Select(f =>
        {
            var d = details.FirstOrDefault(sd => sd.FlowId == f.Id);
            return MapToResponse(f, d);
        }).ToList();
    }

    public async Task<SettlementResponse?> GetByIdAsync(int id)
    {
        var flow = await _wfService.GetFlowByIdAsync(id);
        if (flow == null) return null;
        var detail = await _context.SettlementDetails.FirstOrDefaultAsync(d => d.FlowId == id);
        return MapToResponse(flow, detail);
    }

    // 建造合同三张表
    public async Task<ContractTable> GenerateContractTableAsync(int projectId, int year, int month)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) throw new Exception("Project not found");

        // 自动汇总变更/签证/索赔/认价数据
        var flows = await _context.BusinessFlows
            .Where(f => f.ProjectId == projectId && f.Status == FlowStatus.Archived)
            .ToListAsync();

        var visaTotal = flows.Where(f => f.FlowType == FlowType.Visa).Sum(f => f.Amount ?? 0);
        var claimTotal = flows.Where(f => f.FlowType == FlowType.Claim).Sum(f => f.Amount ?? 0);
        var changeTotal = flows.Where(f => f.FlowType == FlowType.DesignChange).Sum(f => f.Amount ?? 0);

        var settlements = await _context.SettlementDetails
            .Where(d => d.Flow!.ProjectId == projectId).ToListAsync();
        var totalSettlements = settlements.Sum(s => s.TotalAmount ?? s.SubmittedAmount ?? 0);

        var existing = await _context.ContractTables
            .FirstOrDefaultAsync(c => c.ProjectId == projectId && c.Year == year && c.Month == month);

        if (existing != null)
        {
            existing.VisaAdjustment = visaTotal;
            existing.ClaimAdjustment = claimTotal;
            existing.DesignChangeAdjustment = changeTotal;
            existing.MaterialCostActual = totalSettlements;
            existing.AdjustedTotalIncome = existing.OriginalTotalIncome + visaTotal + claimTotal + changeTotal;
            existing.AdjustedTotalCost = existing.OriginalTotalCost + totalSettlements;
            existing.CurrentProfit = existing.AdjustedTotalIncome - existing.AdjustedTotalCost;
            existing.ProfitChangeRate = existing.OriginalProfit > 0
                ? Math.Round((existing.CurrentProfit - existing.OriginalProfit) / existing.OriginalProfit * 100, 2) : 0;
            existing.UpdatedAt = DateTime.Now;
        }
        else
        {
            var contractAmount = project.ContractAmount;
            existing = new ContractTable
            {
                ProjectId = projectId, Year = year, Month = month,
                OriginalTotalIncome = contractAmount,
                VisaAdjustment = visaTotal, ClaimAdjustment = claimTotal, DesignChangeAdjustment = changeTotal,
                AdjustedTotalIncome = contractAmount + visaTotal + claimTotal + changeTotal,
                OriginalTotalCost = contractAmount * 0.85m,
                MaterialCostActual = totalSettlements,
                AdjustedTotalCost = contractAmount * 0.85m + totalSettlements,
                OriginalProfit = contractAmount * 0.15m,
                CurrentProfit = (contractAmount + visaTotal + claimTotal + changeTotal) - (contractAmount * 0.85m + totalSettlements),
                ProfitChangeRate = 0
            };
            existing.ProfitChangeRate = existing.OriginalProfit > 0
                ? Math.Round((existing.CurrentProfit - existing.OriginalProfit) / existing.OriginalProfit * 100, 2) : 0;
            _context.ContractTables.Add(existing);
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<ContractTable?> GetContractTableAsync(int projectId, int year, int month)
    {
        return await _context.ContractTables
            .FirstOrDefaultAsync(c => c.ProjectId == projectId && c.Year == year && c.Month == month);
    }

    private string GetDescription(CreateSettlementRequest r) => r.SettlementType switch
    {
        SettlementType.MonthlyValuation => $"验工月: {r.SettlementMonth}, 完成量: {r.CompletedQuantity}{r.QuantityUnit}, 报审金额: {r.SubmittedAmount}",
        SettlementType.MaterialSettlement => $"材料: {r.MaterialName} {r.MaterialSpec}, 数量: {r.SettlementQuantity}, 分包: {r.Subcontractor}",
        SettlementType.ConsumptionVerification => $"计划: {r.PlannedQuantity}, 实际: {r.ActualQuantity}, 偏差原因: {r.DeviationReason}",
        _ => r.Title
    };

    private SettlementResponse MapToResponse(BusinessFlow f, SettlementDetail? d) => new()
    {
        Id = f.Id, FlowId = f.Id, FlowNo = f.FlowNo,
        SettlementType = d?.SettlementType ?? SettlementType.MonthlyValuation,
        SettlementTypeName = d?.SettlementType switch
        {
            SettlementType.MonthlyValuation => "月验工计价",
            SettlementType.MaterialSettlement => "材料月结算",
            SettlementType.ConsumptionVerification => "月消耗量核定",
            _ => "其他"
        },
        Title = f.Title, Status = f.Status,
        StatusName = f.Status switch
        {
            FlowStatus.Draft => "草稿", FlowStatus.Pending => "待审批",
            FlowStatus.Approved => "已通过", FlowStatus.Rejected => "已驳回",
            FlowStatus.Archived => "已归档", _ => f.Status.ToString()
        },
        Amount = f.Amount,
        SettlementMonth = d?.SettlementMonth,
        CompletedQuantity = d?.CompletedQuantity,
        QuantityUnit = d?.QuantityUnit,
        MaterialName = d?.MaterialName,
        MaterialSpec = d?.MaterialSpec,
        Subcontractor = d?.Subcontractor,
        PlannedQuantity = d?.PlannedQuantity,
        ActualQuantity = d?.ActualQuantity,
        DeviationRate = d?.DeviationRate,
        InitiatedAt = f.InitiatedAt
    };
}