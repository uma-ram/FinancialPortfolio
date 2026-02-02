namespace FinancialPortfolio.Api.Services;

using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs.Requests;
using FinancialPortfolio.Api.Models.DTOs.Responses;

public interface IPortfolioService
{
    Task<PortfolioResponse?> GetPortfolioByIdAsync(int portfolioId);
    Task<IEnumerable<PortfolioResponse>> GetUserPortfoliosAsync(int userId);

    Task<PortfolioResponse> CreatePortfolioAsync(CreatePortfolioRequest request);

    Task<PortfolioResponse?> UpdatePortfolioAsync(int portfolioId, CreatePortfolioRequest request);

    Task<bool> DeletePortfolioAsync(int portfolioId);

    Task<PortfolioSummaryResponse?> GetPortfolioSummaryAsync(int portfolioId);
}
