namespace TXConstructionManagement.Models;

public class SettlementDetail
{
    public int Id { get; set; }
    public int FlowId { get; set; }
    public SettlementType SettlementType { get; set; }

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
    public decimal? TotalAmount { get; set; }
    public string? Subcontractor { get; set; }

    // 消耗量核定
    public decimal? PlannedQuantity { get; set; }
    public decimal? ActualQuantity { get; set; }
    public decimal? DeviationRate { get; set; }
    public string? DeviationReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public BusinessFlow? Flow { get; set; }
}

public enum SettlementType
{
    MonthlyValuation = 0,
    MaterialSettlement = 1,
    ConsumptionVerification = 2
}