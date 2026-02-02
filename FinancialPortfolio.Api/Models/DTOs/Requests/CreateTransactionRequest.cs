using System.ComponentModel.DataAnnotations;

namespace FinancialPortfolio.Api.Models.DTOs.Requests;

public class CreateTransactionRequest
{
    [Required]
    public int AccountId { get; set; }

    [Required]
    [RegularExpression("^(Buy|Sell|Deposit|Withdrawal)$",
        ErrorMessage = "Transaction type must be: Buy, Sell, Deposit, or Withdrawal")]
    public string TransactionType { get; set; } = string.Empty;

    [StringLength(10)]
    public string? Symbol { get; set; } // Required for Buy/Sell

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public decimal Quantity { get; set; } // Shares or amount

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; } // Price per share or total amount

    public DateTime? TransactionDate { get; set; } // Defaults to now if not provided

    [StringLength(500)]
    public string? Notes { get; set; }


}
