
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs.Requests;
using FinancialPortfolio.Api.Models.DTOs.Responses;


namespace FinancialPortfolio.Api.Services;


public interface ITransactionService
{
    Task<TransactionResponse> CreateTransactionAsync(CreateTransactionRequest request);
    Task<IEnumerable<TransactionResponse>> GetAccountTransactionsAsync(int accountId);
    Task<IEnumerable<TransactionResponse>> GetPortfolioTransactionsAsync(int portfolioId);
    Task<TransactionResponse?> GetTransactionByIdAsync(int transactionId);
    Task<bool> DeleteTransactionAsync(int transactionId);
}
