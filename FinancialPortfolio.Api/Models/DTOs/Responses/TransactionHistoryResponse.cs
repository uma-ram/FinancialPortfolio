namespace FinancialPortfolio.Api.Models.DTOs.Responses;

public class TransactionHistoryResponse
{
    public int TotalTransactions { get; set; }
    public List<TransactionHistoryItem> Transactions { get; set; } = new();
    public TransactionSummary Summary { get; set; } = new();
}

public class TransactionHistoryItem
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalAmount { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class TransactionSummary
{
    public int TotalBuys { get; set; }
    public int TotalSells { get; set; }
    public int TotalDeposits { get; set; }
    public int TotalWithdrawals { get; set; }
    public decimal TotalBuyAmount { get; set; }
    public decimal TotalSellAmount { get; set; }
    public decimal TotalDepositAmount { get; set; }
    public decimal TotalWithdrawalAmount { get; set; }
}