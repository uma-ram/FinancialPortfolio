namespace FinancialPortfolio.Api.Services;

using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs.Requests;
using FinancialPortfolio.Api.Models.DTOs.Responses;

public interface IAccountService
{
    Task<AccountResponse?> GetAccountByIdAsync(int accountId);
    Task<IEnumerable<AccountResponse>> GetPortfolioAccountsAsync(int portfolioId);
    Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request);
    Task<bool> DeleteAccountAsync(int accountId);

}
