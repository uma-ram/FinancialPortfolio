namespace FinancialPortfolio.Api.Models.Mongo;

using FinancialPortfolio.Api.Models.DTOs.Responses;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class PortfolioAnalyticsCache
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("portfolioId")]
    public int PortfolioId { get; set; }

    [BsonElement("portfolioName")]
    public string PortfolioName { get; set; } = string.Empty;

    [BsonElement("totalValue")]
    public decimal TotalValue { get; set; }

    [BsonElement("totalCost")]
    public decimal TotalCost { get; set; }

    [BsonElement("totalGainLoss")]
    public decimal TotalGainLoss { get; set; }

    [BsonElement("totalGainLossPercentage")]
    public decimal TotalGainLossPercentage { get; set; }

    [BsonElement("totalReturnOnInvestment")]
    public decimal TotalReturnOnInvestment { get; set; }

    [BsonElement("performance")]
    public PerformanceMetrics Performance { get; set; } = new();

    [BsonElement("holdings")]
    public List<HoldingAnalytics> Holdings { get; set; } = new();

    [BsonElement("assetAllocations")]
    public List<AssetAllocation> AssetAllocations { get; set; } = new();

    [BsonElement("topGainers")]
    public List<TopPerformer> TopGainers { get; set; } = new();

    [BsonElement("topLosers")]
    public List<TopPerformer> TopLosers { get; set; } = new();

    [BsonElement("lastUpdated")]
    public DateTime LastUpdated { get; set; }

    //[BsonElement("totalHoldings")]
    //public int TotalHoldings { get; set; }

    //[BsonElement("lastUpdated")]
    //public DateTime LastUpdated { get; set; }

    //[BsonElement("holdings")]
    //public List<HoldingCache> Holdings { get; set; } = new();
}
public class HoldingCache
{
    [BsonElement("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public decimal Quantity { get; set; }

    [BsonElement("currentValue")]
    public decimal CurrentValue { get; set; }

    [BsonElement("gainLoss")]
    public decimal GainLoss { get; set; }

    [BsonElement("gainLossPercentage")]
    public decimal GainLossPercentage { get; set; }
}

