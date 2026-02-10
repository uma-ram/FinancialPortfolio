using FinancialPortfolio.Api.Models;
using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;

namespace FinancialPortfolio.Api.Repositories;

public class PortfolioRepository : IPortfolioRepository
{

    private readonly FinancialPortfolioDbContext _context;

    public PortfolioRepository(FinancialPortfolioDbContext context)
    {
        _context = context;
    }
    public async Task<Portfolio?> GetByIdAsync(int id)
    {
        return await _context.Portfolios
            .Include(p=>p.Accounts)
            .Include(p=>p.Holdings)
            .FirstOrDefaultAsync(p=>p.Id==id);
    }
    public async Task<IEnumerable<Portfolio>> GetByUserIdAsync(int userId)
    {
        return await _context.Portfolios
             .Where(p => p.UserId == userId)
             .Include(p => p.Accounts)
             .Include(p => p.Holdings)
             .ToListAsync();
    }

    public async Task<Portfolio> AddAsync(Portfolio portfolio)
    {
        _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();
        return portfolio;
    }

    public async Task UpdateAsync(Portfolio portfolio)
    {
        _context.Portfolios.Update(portfolio);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Portfolio portfolio)
    {
        _context.Portfolios.Remove(portfolio);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    
}
