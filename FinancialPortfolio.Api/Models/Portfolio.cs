using System.ComponentModel.DataAnnotations;

namespace FinancialPortfolio.Api.Models;

public class Portfolio
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Navigation Property
    public User User { get; set; } = null!;
    public List<Account> Accounts { get; set; } = new();
    public List<Holding> Holdings { get; set; } = new();


}
