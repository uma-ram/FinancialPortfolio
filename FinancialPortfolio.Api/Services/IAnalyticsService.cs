namespace FinancialPortfolio.Api.Services;
using FinancialPortfolio.Api.Models.DTOs.Responses;
public interface IAnalyticsService
{
    Task<PortfolioAnalyticsResponse?> GetPortfolioAnalyticsAsync(int portfolioId);
    Task<TransactionHistoryResponse> GetTransactionHistoryAsync(int portfolioId, DateTime? startDate = null,
        DateTime? endDate = null, string? transactionType = null);
}
