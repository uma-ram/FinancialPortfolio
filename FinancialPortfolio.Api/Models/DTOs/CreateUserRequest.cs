using System.ComponentModel.DataAnnotations;

namespace FinancialPortfolio.Api.Models.DTOs
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

    }
}
