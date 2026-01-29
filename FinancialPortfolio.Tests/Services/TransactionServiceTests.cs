using Xunit;
using FinancialPortfolio.Api.Services;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;
using FinancialPortfolio.Tests.TestHelpers;

namespace FinancialPortfolio.Tests.Services;

public class TransactionServiceTests
{
    [Fact]
    public async Task CreateTransactionAsync_ShouldCreateBuyTransaction_AndUpdateHoldings()
    {
        // Arrange
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new TransactionService(context);

        var user = new User { Username = "Investor", Email = "investor@example.com", CreatedAt = DateTime.UtcNow };
        var portfolio = new Portfolio { Name = "Growth", UserId = 1, CreatedAt = DateTime.UtcNow };
        var account = new Account { Name = "Stocks", AccountType = "Stocks", PortfolioId = 1, CreatedAt = DateTime.UtcNow };

        context.Users.Add(user);
        context.Portfolios.Add(portfolio);
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var request = new CreateTransactionRequest
        {
            AccountId = account.Id,
            TransactionType = "Buy",
            Symbol = "AAPL",
            Quantity = 10,
            Price = 150.00m
        };

        // Act
        var transaction = await service.CreateTransactionAsync(request);

        // Assert
        Assert.NotNull(transaction);
        Assert.Equal("AAPL", transaction.Symbol);
        Assert.Equal(10, transaction.Quantity);
        Assert.Equal(1500.00m, transaction.TotalAmount);

        // Check holdings were created
        var holding = context.Holdings.FirstOrDefault(h => h.Symbol == "AAPL");
        Assert.NotNull(holding);
        Assert.Equal(10, holding.Quantity);
        Assert.Equal(150.00m, holding.AverageCost);
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldUpdateAverageCost_OnSecondBuy()
    {
        // Arrange
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new TransactionService(context);

        var user = new User { Username = "Test", Email = "test@example.com", CreatedAt = DateTime.UtcNow };
        var portfolio = new Portfolio { Name = "Test", UserId = 1, CreatedAt = DateTime.UtcNow };
        var account = new Account { Name = "Stocks", AccountType = "Stocks", PortfolioId = 1, CreatedAt = DateTime.UtcNow };

        context.Users.Add(user);
        context.Portfolios.Add(portfolio);
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // First buy: 10 shares @ $100
        await service.CreateTransactionAsync(new CreateTransactionRequest
        {
            AccountId = account.Id,
            TransactionType = "Buy",
            Symbol = "MSFT",
            Quantity = 10,
            Price = 100.00m
        });

        // Second buy: 5 shares @ $110
        await service.CreateTransactionAsync(new CreateTransactionRequest
        {
            AccountId = account.Id,
            TransactionType = "Buy",
            Symbol = "MSFT",
            Quantity = 5,
            Price = 110.00m
        });

        // Act
        var holding = context.Holdings.FirstOrDefault(h => h.Symbol == "MSFT");

        // Assert
        Assert.NotNull(holding);
        Assert.Equal(15, holding.Quantity); // 10 + 5
        // Average cost = (10*100 + 5*110) / 15 = 1550 / 15 = 103.33
        Assert.Equal(103.33m, Math.Round(holding.AverageCost, 2));
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldSellShares_AndReduceHoldings()
    {
        // Arrange
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new TransactionService(context);

        var user = new User { Username = "Test", Email = "test@example.com", CreatedAt = DateTime.UtcNow };
        var portfolio = new Portfolio { Name = "Test", UserId = 1, CreatedAt = DateTime.UtcNow };
        var account = new Account { Name = "Stocks", AccountType = "Stocks", PortfolioId = 1, CreatedAt = DateTime.UtcNow };

        context.Users.Add(user);
        context.Portfolios.Add(portfolio);
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Buy 10 shares
        await service.CreateTransactionAsync(new CreateTransactionRequest
        {
            AccountId = account.Id,
            TransactionType = "Buy",
            Symbol = "TSLA",
            Quantity = 10,
            Price = 200.00m
        });

        // Sell 3 shares
        await service.CreateTransactionAsync(new CreateTransactionRequest
        {
            AccountId = account.Id,
            TransactionType = "Sell",
            Symbol = "TSLA",
            Quantity = 3,
            Price = 250.00m
        });

        // Act
        var holding = context.Holdings.FirstOrDefault(h => h.Symbol == "TSLA");

        // Assert
        Assert.NotNull(holding);
        Assert.Equal(7, holding.Quantity); // 10 - 3
    }
    [Fact]
    public async Task CreateTransactionAsync_ShouldThrowException_WhenSellingMoreThanOwned()
    {
        // Arrange
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new TransactionService(context);

        var user = new User { Username = "Test", Email = "test@example.com", CreatedAt = DateTime.UtcNow };
        var portfolio = new Portfolio { Name = "Test", UserId = 1, CreatedAt = DateTime.UtcNow };
        var account = new Account { Name = "Stocks", AccountType = "Stocks", PortfolioId = 1, CreatedAt = DateTime.UtcNow };

        context.Users.Add(user);
        context.Portfolios.Add(portfolio);
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Buy 5 shares
        await service.CreateTransactionAsync(new CreateTransactionRequest
        {
            AccountId = account.Id,
            TransactionType = "Buy",
            Symbol = "GOOGL",
            Quantity = 5,
            Price = 100.00m
        });

        // Try to sell 10 shares (more than owned)
        var sellRequest = new CreateTransactionRequest
        {
            AccountId = account.Id,
            TransactionType = "Sell",
            Symbol = "GOOGL",
            Quantity = 10,
            Price = 120.00m
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateTransactionAsync(sellRequest));
    }
}
