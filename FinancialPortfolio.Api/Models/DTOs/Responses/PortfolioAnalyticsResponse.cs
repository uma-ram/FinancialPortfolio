namespace FinancialPortfolio.Api.Models.DTOs.Responses;

public class PortfolioAnalyticsResponse
{
    public int PortfolioId { get; set; }
    public string PortfolioName { get; set; } = string.Empty;

    // Overall metrics
    public decimal TotalValue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalGainLoss { get; set; }
    public decimal TotalGainLossPercentage { get; set; }
    public decimal TotalReturnOnInvestment { get; set; }

    // Performance metrics
    public PerformanceMetrics Performance { get; set; } = new();

    // Holdings breakdown
    public List<HoldingAnalytics> Holdings { get; set; } = new();

    // Asset allocation
    public List<AssetAllocation> AssetAllocations { get; set; } = new();

    // Top performers
    public List<TopPerformer> TopGainers { get; set; } = new();
    public List<TopPerformer> TopLosers { get; set; } = new();
}

public class PerformanceMetrics
{
    public decimal TotalInvested { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public decimal NetCashFlow { get; set; }
    public int TotalTransactions { get; set; }
    public DateTime? FirstTransactionDate { get; set; }
    public DateTime? LastTransactionDate { get; set; }
    public int DaysActive { get; set; }
}

public class HoldingAnalytics
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal TotalCost { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal GainLoss { get; set; }
    public decimal GainLossPercentage { get; set; }
    public decimal PortfolioWeightPercentage { get; set; }
}

public class AssetAllocation
{
    public string AccountType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Percentage { get; set; }
    public int HoldingsCount { get; set; }
}

public class TopPerformer
{
    public string Symbol { get; set; } = string.Empty;
    public decimal GainLoss { get; set; }
    public decimal GainLossPercentage { get; set; }
    public decimal CurrentValue { get; set; }
}
