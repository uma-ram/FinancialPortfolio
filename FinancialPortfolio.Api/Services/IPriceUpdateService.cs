namespace FinancialPortfolio.Api.Services;

public interface IPriceUpdateService
{
    Task UpdateHoldingPriceAsync(int holdingId, decimal newPrice);
    Task UpdatePortfolioHoldingPricesAsync(int portfolioId, Dictionary<string, decimal> symbolPrices);
    Task<Dictionary<string, decimal>> GetCurrentPricesAsync(IEnumerable<string> symbols);

}
