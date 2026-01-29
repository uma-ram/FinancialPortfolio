namespace FinancialPortfolio.Api.Services;

using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TransactionService: ITransactionService
{
    private readonly FinancialPortfolioDbContext _context;
    public TransactionService(FinancialPortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request)
    {
        // Validate account exists
        var account = await _context.Accounts
            .Include(a => a.Portfolio)
            .FirstOrDefaultAsync(a => a.Id == request.AccountId);

        if (account == null)
        {
            throw new ArgumentException($"Account with ID {request.AccountId} not found");
        }

        // Validate symbol for Buy/Sell transactions
        if ((request.TransactionType == "Buy" || request.TransactionType == "Sell")
            && string.IsNullOrWhiteSpace(request.Symbol))
        {
            throw new ArgumentException("Symbol is required for Buy and Sell transactions");
        }

        // Create transaction
        var transaction = new Transaction
        {
            AccountId = request.AccountId,
            TransactionType = request.TransactionType,
            Symbol = request.Symbol?.ToUpper(),
            Quantity = request.Quantity,
            Price = request.Price,
            TotalAmount = request.Quantity * request.Price,
            TransactionDate = request.TransactionDate ?? DateTime.UtcNow,
            Notes = request.Notes
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Update holdings if it's a Buy or Sell transaction
        if (request.TransactionType == "Buy" || request.TransactionType == "Sell")
        {
            await UpdateHoldingsAsync(account.PortfolioId, transaction);
        }

        return transaction;
    }

    public async Task<bool> DeleteTransactionAsync(int transactionId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction == null)
        {
            return false;
        }

        // If it's a Buy/Sell, we need to recalculate holdings
        var portfolioId = transaction.Account.PortfolioId;
        var symbol = transaction.Symbol;

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        // Recalculate holdings for this symbol
        if (!string.IsNullOrEmpty(symbol))
        {
            await RecalculateHoldingsForSymbolAsync(portfolioId, symbol);
        }

        return true;
    }

    public async Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(int accountId)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetPortfolioTransactionsAsync(int portfolioId)
    {
        return await _context.Transactions
           .Include(t => t.Account)
           .Where(t => t.Account.PortfolioId == portfolioId)
           .OrderByDescending(t => t.TransactionDate)
           .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int transactionId)
    {
        return await _context.Transactions
           .Include(t => t.Account)
           .FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    // CRITICAL METHOD: This is the heart of the portfolio system!
    private async Task UpdateHoldingsAsync(int portfolioId, Transaction transaction)
    {
        if (string.IsNullOrEmpty(transaction.Symbol))
        {
            return;
        }

        var holding = await _context.Holdings
            .FirstOrDefaultAsync(h => h.PortfolioId == portfolioId && h.Symbol == transaction.Symbol);

        if (transaction.TransactionType == "Buy")
        {
            if (holding == null)
            {
                // Create new holding
                holding = new Holding
                {
                    PortfolioId = portfolioId,
                    Symbol = transaction.Symbol,
                    Quantity = transaction.Quantity,
                    AverageCost = transaction.Price,
                    CurrentPrice = transaction.Price, // Initial price
                    LastUpdated = DateTime.UtcNow
                };
                _context.Holdings.Add(holding);
            }
            else
            {
                // Update existing holding with weighted average cost
                var totalCost = (holding.Quantity * holding.AverageCost) + (transaction.Quantity * transaction.Price);
                holding.Quantity += transaction.Quantity;
                holding.AverageCost = totalCost / holding.Quantity;
                holding.LastUpdated = DateTime.UtcNow;
            }
        }
        else if (transaction.TransactionType == "Sell")
        {
            if (holding == null)
            {
                throw new InvalidOperationException($"Cannot sell {transaction.Symbol} - no holdings found");
            }

            if (holding.Quantity < transaction.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient shares. Have {holding.Quantity}, trying to sell {transaction.Quantity}");
            }

            holding.Quantity -= transaction.Quantity;
            holding.LastUpdated = DateTime.UtcNow;

            // Remove holding if quantity is zero
            if (holding.Quantity == 0)
            {
                _context.Holdings.Remove(holding);
            }
        }

        await _context.SaveChangesAsync();
    }
    private async Task RecalculateHoldingsForSymbolAsync(int portfolioId, string symbol)
    {
        // Get all transactions for this symbol in this portfolio
        var transactions = await _context.Transactions
            .Include(t => t.Account)
            .Where(t => t.Account.PortfolioId == portfolioId
                     && t.Symbol == symbol
                     && (t.TransactionType == "Buy" || t.TransactionType == "Sell"))
            .OrderBy(t => t.TransactionDate)
            .ToListAsync();

        // Remove existing holding
        var existingHolding = await _context.Holdings
            .FirstOrDefaultAsync(h => h.PortfolioId == portfolioId && h.Symbol == symbol);

        if (existingHolding != null)
        {
            _context.Holdings.Remove(existingHolding);
        }

        // Recalculate from scratch
        decimal totalQuantity = 0;
        decimal totalCost = 0;
        decimal lastPrice = 0;

        foreach (var txn in transactions)
        {
            if (txn.TransactionType == "Buy")
            {
                totalCost += txn.Quantity * txn.Price;
                totalQuantity += txn.Quantity;
                lastPrice = txn.Price;
            }
            else if (txn.TransactionType == "Sell")
            {
                if (totalQuantity < txn.Quantity)
                {
                    throw new InvalidOperationException("Data integrity error: selling more than owned");
                }

                // Reduce quantity proportionally
                var avgCost = totalQuantity > 0 ? totalCost / totalQuantity : 0;
                totalCost -= txn.Quantity * avgCost;
                totalQuantity -= txn.Quantity;
            }
        }

        // Create new holding if there's still quantity
        if (totalQuantity > 0)
        {
            var newHolding = new Holding
            {
                PortfolioId = portfolioId,
                Symbol = symbol,
                Quantity = totalQuantity,
                AverageCost = totalCost / totalQuantity,
                CurrentPrice = lastPrice,
                LastUpdated = DateTime.UtcNow
            };
            _context.Holdings.Add(newHolding);
        }

        await _context.SaveChangesAsync();
    }
}



