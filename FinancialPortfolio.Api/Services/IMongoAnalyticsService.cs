namespace FinancialPortfolio.Api.Services;

using FinancialPortfolio.Api.Models.Mongo;

public interface IMongoAnalyticsService
{
    Task<PortfolioAnalyticsCache?> GetCachedAnalyticsAsync(int portfolioId);
    Task CacheAnalyticsAsync(PortfolioAnalyticsCache analytics);
    Task InvalidateCacheAsync(int portfolioId);
}
