namespace FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

public class SampleDataSeeder
{
    public static async Task SeedSampleDataAsync(FinancialPortfolioDbContext context)
    {
        // Check if data already exists
        if (context.Users.Any())
        {
            return;
        }

        // Create sample user
        var user = new User
        {
            Username = "Rani Investor",
            Email = "Rani.investor@example.com",
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Create portfolio
        var portfolio = new Portfolio
        {
            Name = "Retirement Portfolio",
            Description = "Long-term retirement savings",
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };
        context.Portfolios.Add(portfolio);
        await context.SaveChangesAsync();

        // Create accounts
        var stockAccount = new Account
        {
            Name = "Stock Investments",
            AccountType = "Stocks",
            PortfolioId = portfolio.Id,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };
        context.Accounts.Add(stockAccount);
        await context.SaveChangesAsync();

        // Create sample transactions
        var transactions = new[]
        {
            new Transaction
            {
                AccountId = stockAccount.Id,
                TransactionType = "Buy",
                Symbol = "AAPL",
                Quantity = 50,
                Price = 145.50m,
                TotalAmount = 7275.00m,
                TransactionDate = DateTime.UtcNow.AddMonths(-5),
                Notes = "Initial Apple investment"
            },
            new Transaction
            {
                AccountId = stockAccount.Id,
                TransactionType = "Buy",
                Symbol = "MSFT",
                Quantity = 30,
                Price = 310.25m,
                TotalAmount = 9307.50m,
                TransactionDate = DateTime.UtcNow.AddMonths(-4),
                Notes = "Microsoft investment"
            },
            new Transaction
            {
                AccountId = stockAccount.Id,
                TransactionType = "Buy",
                Symbol = "GOOGL",
                Quantity = 20,
                Price = 135.75m,
                TotalAmount = 2715.00m,
                TransactionDate = DateTime.UtcNow.AddMonths(-3),
                Notes = "Google investment"
            },
            new Transaction
            {
                AccountId = stockAccount.Id,
                TransactionType = "Buy",
                Symbol = "AAPL",
                Quantity = 25,
                Price = 152.30m,
                TotalAmount = 3807.50m,
                TransactionDate = DateTime.UtcNow.AddMonths(-2),
                Notes = "Additional Apple shares"
            },
            new Transaction
            {
                AccountId = stockAccount.Id,
                TransactionType = "Sell",
                Symbol = "GOOGL",
                Quantity = 5,
                Price = 142.80m,
                TotalAmount = 714.00m,
                TransactionDate = DateTime.UtcNow.AddMonths(-1),
                Notes = "Partial profit taking on Google"
            }
        };
        context.Transactions.AddRange(transactions);
        await context.SaveChangesAsync();

        // Create holdings (calculated from transactions)
        var holdings = new[]
        {
        new Holding
        {
            PortfolioId = portfolio.Id,
            Symbol = "AAPL",
            Quantity = 75, // 50 + 25
            AverageCost = 147.78m, // Weighted average
            CurrentPrice = 175.50m,
            LastUpdated = DateTime.UtcNow
        },
        new Holding
        {
            PortfolioId = portfolio.Id,
            Symbol = "MSFT",
            Quantity = 30,
            AverageCost = 310.25m,
            CurrentPrice = 380.25m,
            LastUpdated = DateTime.UtcNow
        },
        new Holding
        {
            PortfolioId = portfolio.Id,
            Symbol = "GOOGL",
            Quantity = 15, // 20 - 5
            AverageCost = 135.75m,
            CurrentPrice = 142.80m,
            LastUpdated = DateTime.UtcNow
        }
    };

    context.Holdings.AddRange(holdings);
    await context.SaveChangesAsync();
    }
}
