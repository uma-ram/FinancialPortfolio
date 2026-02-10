using System.ComponentModel.DataAnnotations;

namespace FinancialPortfolio.Api.Models;

public class User
{

    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Navigation Property
    public List<Portfolio> Portfolios { get; set; } = new();
}
