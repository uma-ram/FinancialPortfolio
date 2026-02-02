namespace FinancialPortfolio.Api.Models.DTOs.Responses;

public class AccountResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;

    public int PortfolioId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Child resources
    public List<TransactionResponse> Transactions { get; set; } = new();

}
