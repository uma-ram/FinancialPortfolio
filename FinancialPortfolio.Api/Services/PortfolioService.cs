using FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FinancialPortfolio.Api.Services;

public class PortfolioService : IPortfolioService
{
    private readonly FinancialPortfolioDbContext _dbContext;

    public PortfolioService(FinancialPortfolioDbContext dbContext)
    {
        _dbContext = dbContext; 
    }


    public async Task<Portfolio?> GetPortfolioByIdAsync(int portfolioId)
    {
        return await _dbContext.Portfolios
            .Include(p=>p.Accounts)
            .Include(p=>p.Holdings)
            .FirstOrDefaultAsync(p=>p.Id==portfolioId);
    }

    public async Task<IEnumerable<Portfolio>> GetUserPortfoliosAsync(int userId)
    {
        return await _dbContext.Portfolios
            .Where(u => u.Id == userId)
            .Include(p => p.Accounts)
            .Include(p => p.Holdings)
            .ToListAsync();
    }

    public async Task<Portfolio> CreatePortfolioAsync(CreatePortfolioRequest request)
    {
        //Verify that the user exists
        var userExists = await _dbContext.Users.AnyAsync(u=>u.Id==request.UserId);
        if (!userExists)
        {
            throw new ArgumentException($"User with ID - {request.UserId} not found"); 
        }

        var portfolio = new Portfolio
        {
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Portfolios.Add(portfolio);
        await _dbContext.SaveChangesAsync();

        return portfolio;
    }

    public async Task<Portfolio?> UpdatePortfolioAsync(int portfolioId, CreatePortfolioRequest request)
    {
        var portfolio = await _dbContext.Portfolios.FindAsync(portfolioId);
        if(portfolio == null)
        {
            return null;
        }
        portfolio.Name = request.Name;
        portfolio.Description = request.Description;

        await _dbContext.SaveChangesAsync();
        return portfolio;
    }

    public async Task<bool> DeletePortfolioAsync(int portfolioId)
    {
        var portfolio = await _dbContext.Portfolios.FindAsync(portfolioId);
        if (portfolio == null)
        {
            return false;
        }
        _dbContext.Portfolios.Remove(portfolio);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<PortfolioSummaryResponse?> GetPortfolioSummaryAsync(int portfolioId)
    {
        var portfolio = await _dbContext.Portfolios
           .Include(p => p.Holdings)
           .FirstOrDefaultAsync(p => p.Id == portfolioId);

        if (portfolio == null)
        {
            return null;
        }
        var holdings = portfolio.Holdings.Select(h => new HoldingSummary
        {
            Symbol = h.Symbol,
            Quantity = h.Quantity,
            AverageCost = h.AverageCost,
            CurrentPrice = h.CurrentPrice,
            CurrentValue = h.CurrentValue,
            GainLoss = h.GainLoss,
            GainLossPercentage = h.GainLossPercentage
        }).ToList();

        var totalValue = holdings.Sum(h => h.CurrentValue);
        var totalCost = holdings.Sum(h => h.Quantity * h.AverageCost);
        var totalGainLoss = totalValue - totalCost;

        return new PortfolioSummaryResponse
        {
            PortfolioId = portfolio.Id,
            PortfolioName = portfolio.Name,
            TotalValue = totalValue,
            TotalCost = totalCost,
            TotalGainLoss = totalGainLoss,
            TotalGainLossPercentage = totalCost > 0 ? (totalGainLoss / totalCost) * 100 : 0,
            TotalHoldings = holdings.Count,
            Holdings = holdings
        };
    }    
}
