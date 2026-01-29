using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;

namespace FinancialPortfolio.Api.Services;

public class AccountService : IAccountService
{
    private readonly FinancialPortfolioDbContext _context;

    public AccountService(FinancialPortfolioDbContext context)
    {
        _context = context;
    }
    public async Task<Account> CreateAccountAsync(CreateAccountRequest request)
    {
        // Verify portfolio exists
        var portfolioExists = await _context.Portfolios.AnyAsync(p => p.Id == request.PortfolioId);
        if (!portfolioExists)
        {
            throw new ArgumentException($"Portfolio with ID {request.PortfolioId} not found");
        }

        var account = new Account
        {
            Name = request.Name,
            AccountType = request.AccountType,
            PortfolioId = request.PortfolioId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }

    public async Task<bool> DeleteAccountAsync(int accountId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            return false;
        }

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId)
    {
        return await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == accountId);
    }

    public async Task<IEnumerable<Account>> GetPortfolioAccountsAsync(int portfolioId)
    {
        return await _context.Accounts
           .Where(a => a.PortfolioId == portfolioId)
           .Include(a => a.Transactions)
           .ToListAsync();
    }
}
