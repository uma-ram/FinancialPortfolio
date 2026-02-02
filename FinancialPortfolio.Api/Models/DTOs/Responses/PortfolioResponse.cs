namespace FinancialPortfolio.Api.Models.DTOs.Responses
{
    public class PortfolioResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        // Child resources
        public List<AccountResponse> Accounts { get; set; } = new();

        public List<HoldingResponse> Holdings { get; set; } = new();

    }
}
