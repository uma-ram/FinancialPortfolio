namespace FinancialPortfolio.Api.Models.DTOs.Responses;

public class HoldingResponse
{
    public int Id { get; set; }

    public int PortfolioId { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal AverageCost { get; set; }

    public decimal CurrentPrice { get; set; }

    public DateTime LastUpdated { get; set; }

    // Calculated values (API-friendly)
    public decimal TotalCost { get; set; }

    public decimal CurrentValue { get; set; }

    public decimal GainLoss { get; set; }

    public decimal GainLossPercentage { get; set; }
}
