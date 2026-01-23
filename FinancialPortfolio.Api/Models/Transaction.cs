using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialPortfolio.Api.Models;

public class Transaction
{
    [Required]
    [StringLength(20)]
    public string TransactionType { get; set; } = string.Empty; // Buy, Sell, Deposit, Withdrawal

    [StringLength(10)]
    public string? Symbol { get; set; } // Stock symbol (e.g., AAPL, MSFT) - null for deposits/withdrawals

    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; } // Shares or amount

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } // Price per share or total amount

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } // Quantity * Price

    public DateTime TransactionDate { get; set; }

    public string? Notes { get; set; }

    // Navigation property
    public Account Account { get; set; } = null!;
}
