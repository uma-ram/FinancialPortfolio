namespace FinancialPortfolio.Api.Models.DTOs.Responses;

public class TransactionResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public string? Symbol { get; set; }

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime TransactionDate { get; set; }

    public string? Notes { get; set; }
}
