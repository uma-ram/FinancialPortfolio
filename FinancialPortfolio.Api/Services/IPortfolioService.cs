namespace FinancialPortfolio.Api.Services;

using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;


public interface IPortfolioService
{
    Task<Portfolio?> GetPortfolioByIdAsync(int portfolioId);
    Task<IEnumerable<Portfolio>> GetUserPortfoliosAsync(int userId);

    Task<Portfolio> CreatePortfolioAsync(CreatePortfolioRequest request);

    Task<Portfolio?> UpdatePortfolioAsync(int portfolioId, CreatePortfolioRequest request);

    Task<bool> DeletePortfolioAsync(int portfolioId);

    Task<PortfolioSummaryResponse?> GetPortfolioSummaryAsync(int portfolioId);
}
