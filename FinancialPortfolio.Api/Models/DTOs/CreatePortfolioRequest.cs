

using System.ComponentModel.DataAnnotations;

namespace FinancialPortfolio.Api.Models.DTOs;


public class CreatePortfolioRequest
{
    [Required(ErrorMessage="Portfolio name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public int UserId { get; set; }
}
