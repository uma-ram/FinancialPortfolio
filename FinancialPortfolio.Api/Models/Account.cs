using System.ComponentModel.DataAnnotations;

namespace FinancialPortfolio.Api.Models;

public class Account
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string AccountType { get; set; } = string.Empty; // Stocks, Bonds, Cash, Crypto

    public int PortfolioId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Portfolio Portfolio { get; set; } = null!;
    public List<Transaction> Transactions { get; set; } = new();
}
