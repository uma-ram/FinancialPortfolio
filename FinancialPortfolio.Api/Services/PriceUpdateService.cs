namespace FinancialPortfolio.Api.Services;

using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

public class PriceUpdateService : IPriceUpdateService
{
    private readonly FinancialPortfolioDbContext _context;
    private readonly ILogger<PriceUpdateService> _logger;
    public PriceUpdateService(
        FinancialPortfolioDbContext context,
        ILogger<PriceUpdateService> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<Dictionary<string, decimal>> GetCurrentPricesAsync(IEnumerable<string> symbols)
    {
        // Simulate API delay
        await Task.Delay(100);

        // Return simulated prices (in production, call real API)
        var prices = new Dictionary<string, decimal>();
        var random = new Random();

        foreach (var symbol in symbols)
        {
            // Generate random price between $50 and $500
            var price = random.Next(50, 500) + (decimal)random.NextDouble();
            prices[symbol] = Math.Round(price, 2);
        }

        return prices;
    }

    public async Task UpdateHoldingPriceAsync(int holdingId, decimal newPrice)
    {
        var holding = await _context.Holdings.FindAsync(holdingId);
        if (holding == null)
        {
            throw new ArgumentException($"Holding with Id {holdingId} not found");
        }
        if (newPrice <= 0)
        {
            throw new ArgumentException("Price must be greater than 0");
        }
        holding.CurrentPrice = newPrice;
        holding.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated holding {HoldingId} ({Symbol}) price to {NewPrice}",
            holdingId, holding.Symbol, newPrice);
    }

    public async Task UpdatePortfolioHoldingPricesAsync(int portfolioId, Dictionary<string, decimal> symbolPrices)
    {
        var holdings = await _context.Holdings
                        .Where(x => x.Id == portfolioId)
                        .ToListAsync();
        var updatedCount = 0;
        foreach (var holding in holdings)
        {
            if (symbolPrices.TryGetValue(holding.Symbol, out var newprice))
            {
                if (newprice > 0)
                {
                    holding.CurrentPrice= newprice;
                    holding.LastUpdated = DateTime.UtcNow;
                    updatedCount++;
                }
            }
        }
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated {Count} holdings in portfolio {PortfolioId}",
            updatedCount, portfolioId);
    }
}
