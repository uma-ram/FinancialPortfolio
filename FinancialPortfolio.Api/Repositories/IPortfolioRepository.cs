namespace FinancialPortfolio.Api.Repositories;
using FinancialPortfolio.Api.Models;
public interface IPortfolioRepository
{
    Task<Portfolio?> GetByIdAsync(int id);
    Task<IEnumerable<Portfolio>> GetByUserIdAsync(int userId);
    Task<Portfolio> AddAsync(Portfolio portfolio);
    Task UpdateAsync(Portfolio portfolio);
    Task DeleteAsync(Portfolio portfolio);
    Task<bool> SaveChangesAsync();
}
