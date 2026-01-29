namespace FinancialPortfolio.Api.Services;

using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;
public interface IAccountService
{
    Task<Account?> GetAccountByIdAsync(int accountId);
    Task<IEnumerable<Account>> GetPortfolioAccountsAsync(int portfolioId);
    Task<Account> CreateAccountAsync(CreateAccountRequest request);
    Task<bool> DeleteAccountAsync(int accountId);

}
