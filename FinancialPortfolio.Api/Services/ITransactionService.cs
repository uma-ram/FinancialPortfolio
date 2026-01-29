
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;


namespace FinancialPortfolio.Api.Services;


public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request);
    Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(int accountId);
    Task<IEnumerable<Transaction>> GetPortfolioTransactionsAsync(int portfolioId);
    Task<Transaction?> GetTransactionByIdAsync(int transactionId);
    Task<bool> DeleteTransactionAsync(int transactionId);
}
