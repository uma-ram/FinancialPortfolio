namespace FinancialPortfolio.Api.Models.DTOs.Requests;

public class UpdatePriceRequest
{
    public decimal NewPrice { get; set; }
}

public class UpdatePortfolioPricesRequest
{
    public Dictionary<string, decimal> SymbolPrices { get; set; } = new();
}

public class FetchPricesRequest
{
    public List<string> Symbols { get; set; } = new();
}