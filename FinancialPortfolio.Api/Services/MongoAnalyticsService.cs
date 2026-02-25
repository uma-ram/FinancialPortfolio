namespace FinancialPortfolio.Api.Services;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.Mongo;
using MongoDB.Driver;
using System.Threading.Tasks;

public class MongoAnalyticsService : IMongoAnalyticsService
{
    private readonly IMongoCollection<PortfolioAnalyticsCache> _collection;
    private readonly ILogger<MongoAnalyticsService> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public MongoAnalyticsService(
        IMongoClient mongoClient,
        IConfiguration configuration,
        ILogger<MongoAnalyticsService> logger)
    {
        var database = mongoClient.GetDatabase("FinancialPortfolio");
        _collection = database.GetCollection<PortfolioAnalyticsCache>("PortfolioAnalytics");
        _logger = logger;

        // Create index on portfolioId for faster lookups
        var indexKeys = Builders<PortfolioAnalyticsCache>.IndexKeys.Ascending(x => x.PortfolioId);
        var indexModel = new CreateIndexModel<PortfolioAnalyticsCache>(indexKeys);
        _collection.Indexes.CreateOne(indexModel);

    }
    public async Task CacheAnalyticsAsync(PortfolioAnalyticsCache analytics)
    {
        try
        {
            analytics.LastUpdated = DateTime.UtcNow;

            var filter = Builders<PortfolioAnalyticsCache>.Filter.Eq(x => x.PortfolioId, analytics.PortfolioId);
            var options = new ReplaceOptions { IsUpsert = true };

            await _collection.ReplaceOneAsync(filter, analytics, options);

            _logger.LogInformation("Cached analytics for portfolio {PortfolioId}", analytics.PortfolioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching analytics for portfolio {PortfolioId}", analytics.PortfolioId);
            // Don't throw - caching is optional
        }
    }

    public async Task<PortfolioAnalyticsCache?> GetCachedAnalyticsAsync(int portfolioId)
    {
        try
        {
            var cached = await _collection
                .Find(x => x.PortfolioId == portfolioId)
                .FirstOrDefaultAsync();

            if (cached == null)
            {
                _logger.LogInformation("Cache miss for portfolio {PortfolioId}", portfolioId);
                return null;
            }

            // Check if cache is expired
            if (DateTime.UtcNow - cached.LastUpdated > _cacheExpiration)
            {
                _logger.LogInformation("Cache expired for portfolio {PortfolioId}", portfolioId);
                await InvalidateCacheAsync(portfolioId);
                return null;
            }

            _logger.LogInformation("Cache hit for portfolio {PortfolioId}", portfolioId);
            return cached;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached analytics for portfolio {PortfolioId}", portfolioId);
            return null; // Fall back to database on error
        }
    }

    public async Task InvalidateCacheAsync(int portfolioId)
    {
        try
        {
            await _collection.DeleteOneAsync(x => x.PortfolioId == portfolioId);
            _logger.LogInformation("Invalidated cache for portfolio {PortfolioId}", portfolioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache for portfolio {PortfolioId}", portfolioId);
        }
    }
}
