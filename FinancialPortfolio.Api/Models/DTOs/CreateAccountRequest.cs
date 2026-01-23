using System.ComponentModel.DataAnnotations;

namespace FinancialPortfolio.Api.Models.DTOs;

public class CreateAccountRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Stocks|Bonds|Cash|Crypto)$", 
        ErrorMessage = "AccountType must be one of the following: Stocks, Bonds, Cash, Crypto.")]
    public string AccountType { get; set; } = string.Empty;

    [Required]
    public int PortfolioId { get; set; }

}
