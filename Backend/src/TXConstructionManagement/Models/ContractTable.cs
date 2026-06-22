namespace TXConstructionManagement.Models;

public class ContractTable
{
    public int Id { get; set; }
    public int ProjectId { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }

    // 预计总收入调整表
    public decimal OriginalTotalIncome { get; set; }
    public decimal VisaAdjustment { get; set; }
    public decimal ClaimAdjustment { get; set; }
    public decimal DesignChangeAdjustment { get; set; }
    public decimal OtherIncomeAdjustment { get; set; }
    public decimal AdjustedTotalIncome { get; set; }
    public string IncomeAdjustReason { get; set; } = string.Empty;

    // 预计总成本调整表
    public decimal OriginalTotalCost { get; set; }
    public decimal MaterialCostActual { get; set; }
    public decimal LaborCostActual { get; set; }
    public decimal OtherCostActual { get; set; }
    public decimal AdjustedTotalCost { get; set; }
    public string CostAdjustReason { get; set; } = string.Empty;

    // 利润动态调整表
    public decimal OriginalProfit { get; set; }
    public decimal CurrentProfit { get; set; }
    public decimal ProfitChangeRate { get; set; }
    public string ProfitAnalysis { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Project? Project { get; set; }
}