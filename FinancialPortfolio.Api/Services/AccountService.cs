using FinancialPortfolio.Api.Models;
using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Models.DTOs.Requests;
using FinancialPortfolio.Api.Models.DTOs.Responses;
using AutoMapper;


namespace FinancialPortfolio.Api.Services;

public class AccountService : IAccountService
{
    private readonly FinancialPortfolioDbContext _context;
    private  readonly IMapper _mapper;

    public AccountService(FinancialPortfolioDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request)
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

        return _mapper.Map<AccountResponse>(account);
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

    public async Task<AccountResponse?> GetAccountByIdAsync(int accountId)
    {
        var response =  await _context.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == accountId);
        return _mapper.Map<AccountResponse>(response);
    }

    public async Task<IEnumerable<AccountResponse>> GetPortfolioAccountsAsync(int portfolioId)
    {
        var response = await _context.Accounts
           .Where(a => a.PortfolioId == portfolioId)
           .Include(a => a.Transactions)
           .ToListAsync();
        return _mapper.Map<IEnumerable<AccountResponse>>(response.ToList());
    }
}
