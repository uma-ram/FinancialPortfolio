using Xunit;
using FinancialPortfolio.Api.Services;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Tests.TestHelpers;

namespace FinancialPortfolio.Tests.Services;

public class AnalyticsServiceTests
{
    [Fact]
    public async Task GetPortfolioAnalytics_ShouldCalculateMetricsCorrectly()
    {
        // Arrange
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new AnalyticsService(context);

        var user = new User { Username = "Test", Email = "test@test.com", CreatedAt = DateTime.UtcNow };
        var portfolio = new Portfolio { Name = "Test", UserId = 1, CreatedAt = DateTime.UtcNow };
        var account = new Account { Name = "Stocks", AccountType = "Stocks", PortfolioId = 1, CreatedAt = DateTime.UtcNow };

        context.Users.Add(user);
        context.Portfolios.Add(portfolio);
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Add holding
        var holding = new Holding
        {
            PortfolioId = portfolio.Id,
            Symbol = "AAPL",
            Quantity = 10,
            AverageCost = 100.00m,
            CurrentPrice = 120.00m,
            LastUpdated = DateTime.UtcNow
        };
        context.Holdings.Add(holding);

        // Add transaction
        var transaction = new Transaction
        {
            AccountId = account.Id,
            TransactionType = "Buy",
            Symbol = "AAPL",
            Quantity = 10,
            Price = 100.00m,
            TotalAmount = 1000.00m,
            TransactionDate = DateTime.UtcNow.AddDays(-30)
        };
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        // Act
        var analytics = await service.GetPortfolioAnalyticsAsync(portfolio.Id);

        // Assert
        Assert.NotNull(analytics);
        Assert.Equal(1200.00m, analytics.TotalValue); // 10 * 120
        Assert.Equal(1000.00m, analytics.TotalCost); // 10 * 100
        Assert.Equal(200.00m, analytics.TotalGainLoss);
        Assert.Equal(20.00m, analytics.TotalGainLossPercentage);
        Assert.Single(analytics.Holdings);
        Assert.Equal(1, analytics.Performance.TotalTransactions);
    }

    [Fact]
    public async Task GetTransactionHistory_ShouldFilterByType()
    {
        // Arrange
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new AnalyticsService(context);

        var user = new User { Username = "Test", Email = "test@test.com", CreatedAt = DateTime.UtcNow };
        var portfolio = new Portfolio { Name = "Test", UserId = 1, CreatedAt = DateTime.UtcNow };
        var account = new Account { Name = "Stocks", AccountType = "Stocks", PortfolioId = 1, CreatedAt = DateTime.UtcNow };

        context.Users.Add(user);
        context.Portfolios.Add(portfolio);
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Add multiple transactions
        context.Transactions.AddRange(
            new Transaction { AccountId = account.Id, TransactionType = "Buy", Symbol = "AAPL", Quantity = 10, Price = 100, TotalAmount = 1000, TransactionDate = DateTime.UtcNow },
            new Transaction { AccountId = account.Id, TransactionType = "Sell", Symbol = "AAPL", Quantity = 5, Price = 110, TotalAmount = 550, TransactionDate = DateTime.UtcNow },
            new Transaction { AccountId = account.Id, TransactionType = "Buy", Symbol = "MSFT", Quantity = 5, Price = 200, TotalAmount = 1000, TransactionDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        // Act
        var history = await service.GetTransactionHistoryAsync(portfolio.Id, transactionType: "Buy");

        // Assert
        Assert.Equal(2, history.TotalTransactions);
        Assert.All(history.Transactions, t => Assert.Equal("Buy", t.TransactionType));
        Assert.Equal(2, history.Summary.TotalBuys);
        Assert.Equal(1, history.Summary.TotalSells);
    }
}
