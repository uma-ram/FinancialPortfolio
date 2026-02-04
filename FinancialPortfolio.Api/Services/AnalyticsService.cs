using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Models.DTOs.Requests;
using FinancialPortfolio.Api.Models.DTOs.Responses;

namespace FinancialPortfolio.Api.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly FinancialPortfolioDbContext _context;

    public AnalyticsService(FinancialPortfolioDbContext context)
    {
        _context = context;
    }
    public async Task<PortfolioAnalyticsResponse?> GetPortfolioAnalyticsAsync(int portfolioId)
    {
        var portfolio = await _context.Portfolios
           .Include(p => p.Holdings)
           .Include(p => p.Accounts)
               .ThenInclude(a => a.Transactions)
           .FirstOrDefaultAsync(p => p.Id == portfolioId);

        if (portfolio == null)
        {
            return null;
        }

        var allTransactions = portfolio.Accounts
            .SelectMany(a => a.Transactions)
            .ToList();

        // Calculate overall metrics
        var totalValue = portfolio.Holdings.Sum(h => h.CurrentValue);
        var totalCost = portfolio.Holdings.Sum(h => h.TotalCost);
        var totalGainLoss = totalValue - totalCost;
        var totalGainLossPercentage = totalCost > 0 ? (totalGainLoss / totalCost) * 100 : 0;

        // Calculate performance metrics
        var buyTransactions = allTransactions.Where(t => t.TransactionType == "Buy").ToList();
        var sellTransactions = allTransactions.Where(t => t.TransactionType == "Sell").ToList();
        var depositTransactions = allTransactions.Where(t => t.TransactionType == "Deposit").ToList();
        var withdrawalTransactions = allTransactions.Where(t => t.TransactionType == "Withdrawal").ToList();

        var totalInvested = buyTransactions.Sum(t => t.TotalAmount) + depositTransactions.Sum(t => t.TotalAmount);
        var totalWithdrawn = sellTransactions.Sum(t => t.TotalAmount) + withdrawalTransactions.Sum(t => t.TotalAmount);

        var firstTransaction = allTransactions.OrderBy(t => t.TransactionDate).FirstOrDefault();
        var lastTransaction = allTransactions.OrderByDescending(t => t.TransactionDate).FirstOrDefault();

        var daysActive = firstTransaction != null && lastTransaction != null
            ? (lastTransaction.TransactionDate - firstTransaction.TransactionDate).Days
            : 0;

        var performance = new PerformanceMetrics
        {
            TotalInvested = totalInvested,
            TotalWithdrawn = totalWithdrawn,
            NetCashFlow = totalInvested - totalWithdrawn,
            TotalTransactions = allTransactions.Count,
            FirstTransactionDate = firstTransaction?.TransactionDate,
            LastTransactionDate = lastTransaction?.TransactionDate,
            DaysActive = daysActive
        };

        // Holdings analytics
        var holdingAnalytics = portfolio.Holdings.Select(h => new HoldingAnalytics
        {
            Symbol = h.Symbol,
            Quantity = h.Quantity,
            AverageCost = h.AverageCost,
            CurrentPrice = h.CurrentPrice,
            TotalCost = h.TotalCost,
            CurrentValue = h.CurrentValue,
            GainLoss = h.GainLoss,
            GainLossPercentage = h.GainLossPercentage,
            PortfolioWeightPercentage = totalValue > 0 ? (h.CurrentValue / totalValue) * 100 : 0
        }).OrderByDescending(h => h.CurrentValue).ToList();

        // Asset allocation (by account type)
        var accountGroups = portfolio.Accounts
            .GroupBy(a => a.AccountType)
            .Select(g => new
            {
                AccountType = g.Key,
                Accounts = g.ToList()
            })
            .ToList();

        var assetAllocations = new List<AssetAllocation>();
        foreach (var group in accountGroups)
        {
            var accountIds = group.Accounts.Select(a => a.Id).ToList();
            var accountTransactions = allTransactions.Where(t => accountIds.Contains(t.AccountId)).ToList();

            // Calculate value for this account type
            var accountValue = portfolio.Holdings
                .Where(h => accountTransactions.Any(t => t.Symbol == h.Symbol))
                .Sum(h => h.CurrentValue);

            assetAllocations.Add(new AssetAllocation
            {
                AccountType = group.AccountType,
                Value = accountValue,
                Percentage = totalValue > 0 ? (accountValue / totalValue) * 100 : 0,
                HoldingsCount = portfolio.Holdings.Count(h => accountTransactions.Any(t => t.Symbol == h.Symbol))
            });
        }

        // Top gainers and losers
        var topGainers = holdingAnalytics
            .Where(h => h.GainLoss > 0)
            .OrderByDescending(h => h.GainLossPercentage)
            .Take(5)
            .Select(h => new TopPerformer
            {
                Symbol = h.Symbol,
                GainLoss = h.GainLoss,
                GainLossPercentage = h.GainLossPercentage,
                CurrentValue = h.CurrentValue
            })
            .ToList();

        var topLosers = holdingAnalytics
            .Where(h => h.GainLoss < 0)
            .OrderBy(h => h.GainLossPercentage)
            .Take(5)
            .Select(h => new TopPerformer
            {
                Symbol = h.Symbol,
                GainLoss = h.GainLoss,
                GainLossPercentage = h.GainLossPercentage,
                CurrentValue = h.CurrentValue
            })
            .ToList();

        return new PortfolioAnalyticsResponse
        {
            PortfolioId = portfolio.Id,
            PortfolioName = portfolio.Name,
            TotalValue = totalValue,
            TotalCost = totalCost,
            TotalGainLoss = totalGainLoss,
            TotalGainLossPercentage = totalGainLossPercentage,
            TotalReturnOnInvestment = totalInvested > 0 ? (totalGainLoss / totalInvested) * 100 : 0,
            Performance = performance,
            Holdings = holdingAnalytics,
            AssetAllocations = assetAllocations,
            TopGainers = topGainers,
            TopLosers = topLosers
        };
    }

    public async Task<TransactionHistoryResponse> GetTransactionHistoryAsync(int portfolioId, DateTime? startDate = null, DateTime? endDate = null, string? transactionType = null)
    {
        var query = _context.Transactions
            .Include(t => t.Account)
            .Where(t => t.Account.PortfolioId == portfolioId);

        // Apply filters
        if (startDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= endDate.Value);
        }

        if (!string.IsNullOrEmpty(transactionType))
        {
            query = query.Where(t => t.TransactionType == transactionType);
        }

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        var transactionItems = transactions.Select(t => new TransactionHistoryItem
        {
            Id = t.Id,
            TransactionDate = t.TransactionDate,
            TransactionType = t.TransactionType,
            Symbol = t.Symbol,
            Quantity = t.Quantity,
            Price = t.Price,
            TotalAmount = t.TotalAmount,
            AccountName = t.Account.Name,
            Notes = t.Notes
        }).ToList();

        var summary = new TransactionSummary
        {
            TotalBuys = transactions.Count(t => t.TransactionType == "Buy"),
            TotalSells = transactions.Count(t => t.TransactionType == "Sell"),
            TotalDeposits = transactions.Count(t => t.TransactionType == "Deposit"),
            TotalWithdrawals = transactions.Count(t => t.TransactionType == "Withdrawal"),
            TotalBuyAmount = transactions.Where(t => t.TransactionType == "Buy").Sum(t => t.TotalAmount),
            TotalSellAmount = transactions.Where(t => t.TransactionType == "Sell").Sum(t => t.TotalAmount),
            TotalDepositAmount = transactions.Where(t => t.TransactionType == "Deposit").Sum(t => t.TotalAmount),
            TotalWithdrawalAmount = transactions.Where(t => t.TransactionType == "Withdrawal").Sum(t => t.TotalAmount)
        };

        return new TransactionHistoryResponse
        {
            TotalTransactions = transactions.Count,
            Transactions = transactionItems,
            Summary = summary
        };
    }
}
