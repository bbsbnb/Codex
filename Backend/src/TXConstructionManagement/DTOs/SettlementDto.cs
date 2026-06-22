using TXConstructionManagement.Models;

namespace TXConstructionManagement.DTOs;

public class CreateSettlementRequest
{
    public int ProjectId { get; set; }
    public SettlementType SettlementType { get; set; }
    public string Title { get; set; } = string.Empty;

    // 月验工计价
    public string? SettlementMonth { get; set; }
    public decimal? CompletedQuantity { get; set; }
    public string? QuantityUnit { get; set; }
    public decimal? SubmittedAmount { get; set; }

    // 材料月结算
    public string? MaterialName { get; set; }
    public string? MaterialSpec { get; set; }
    public decimal? SettlementQuantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Subcontractor { get; set; }

    // 消耗量核定
    public decimal? PlannedQuantity { get; set; }
    public decimal? ActualQuantity { get; set; }
    public string? DeviationReason { get; set; }
}

public class SettlementResponse
{
    public int Id { get; set; }
    public int FlowId { get; set; }
    public string FlowNo { get; set; } = string.Empty;
    public SettlementType SettlementType { get; set; }
    public string SettlementTypeName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public FlowStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public decimal? Amount { get; set; }

    // 月验工计价
    public string? SettlementMonth { get; set; }
    public decimal? CompletedQuantity { get; set; }
    public string? QuantityUnit { get; set; }

    // 材料月结算
    public string? MaterialName { get; set; }
    public string? MaterialSpec { get; set; }
    public string? Subcontractor { get; set; }

    // 消耗量核定
    public decimal? PlannedQuantity { get; set; }
    public decimal? ActualQuantity { get; set; }
    public decimal? DeviationRate { get; set; }

    public DateTime InitiatedAt { get; set; }
}

public class ContractTableResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;

    // 预计总收入调整表
    public decimal OriginalTotalIncome { get; set; }
    public decimal VisaAdjustment { get; set; }
    public decimal ClaimAdjustment { get; set; }
    public decimal DesignChangeAdjustment { get; set; }
    public decimal AdjustedTotalIncome { get; set; }

    // 预计总成本调整表
    public decimal OriginalTotalCost { get; set; }
    public decimal MaterialCostActual { get; set; }
    public decimal AdjustedTotalCost { get; set; }

    // 利润动态调整表
    public decimal OriginalProfit { get; set; }
    public decimal CurrentProfit { get; set; }
    public decimal ProfitChangeRate { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class UpdateContractTableRequest
{
    public decimal? VisaAdjustment { get; set; }
    public decimal? ClaimAdjustment { get; set; }
    public decimal? DesignChangeAdjustment { get; set; }
    public decimal? MaterialCostActual { get; set; }
    public decimal? LaborCostActual { get; set; }
    public decimal? OtherCostActual { get; set; }
    public string? IncomeAdjustReason { get; set; }
    public string? CostAdjustReason { get; set; }
    public string? ProfitAnalysis { get; set; }
}