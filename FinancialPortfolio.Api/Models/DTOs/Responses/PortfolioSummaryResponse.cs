namespace FinancialPortfolio.Api.Models.DTOs.Responses;

public class PortfolioSummaryResponse
{
    public int PortfolioId { get; set; }
    public string PortfolioName { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalGainLoss { get; set; }
    public decimal TotalGainLossPercentage { get; set; }
    public int TotalHoldings { get; set; }
    public List<HoldingSummary> Holdings { get; set; } = new();
}

public class HoldingSummary
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal GainLoss { get; set; }
    public decimal GainLossPercentage { get; set; }
}