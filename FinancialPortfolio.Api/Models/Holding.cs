using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialPortfolio.Api.Models
{
    public class Holding
    {
        public int Id { get; set; }

        public int PortfolioId { get; set; }

        [Required]
        [StringLength(10)]
        public string Symbol { get; set; } = string.Empty; // Stock symbol

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; } // Total shares owned

        [Column(TypeName = "decimal(18,2)")]
        public decimal AverageCost { get; set; } // Average purchase price

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentPrice { get; set; } // Current market price (updated periodically)

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Calculated properties
        [NotMapped]
        public decimal TotalCost => Quantity * AverageCost;

        [NotMapped]
        public decimal CurrentValue => Quantity * CurrentPrice;

        [NotMapped]
        public decimal GainLoss => CurrentValue - TotalCost;

        [NotMapped]
        public decimal GainLossPercentage => TotalCost > 0 ? (GainLoss / TotalCost) * 100 : 0;

        // Navigation property
        public Portfolio Portfolio { get; set; } = null!;
    }
}
