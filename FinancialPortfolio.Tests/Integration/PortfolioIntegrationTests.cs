using FinancialPortfolio.Api.Controllers;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs.Requests;
using FinancialPortfolio.Api.Models.DTOs.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FinancialPortfolio.Tests.Integration;

public class PortfolioIntegrationTests:IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PortfolioIntegrationTests(WebApplicationFactory<Program> applicationFactory)
    {
        _client = applicationFactory.CreateClient();
    }

    [Fact]
    public async Task CompleteWorkflow_ShouldWork_FromUserToTransaction()
    {
        // 1. Create User
        var createUserRequest = new CreateUserRequest
        {
            Name = "Test User",
            Email = "test@integration.com"
        };

        var userResponse = await _client.PostAsJsonAsync("/api/users", createUserRequest);
        Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);

        var user = await userResponse.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(user);
        Assert.Equal("Test User", user.Username);

        // 2. Create Portfolio
        var createPortfolioRequest = new CreatePortfolioRequest
        {
            Name = "Integration Test Portfolio",
            UserId = user.Id
        };

        var portfolioResponse = await _client.PostAsJsonAsync("/api/portfolios", createPortfolioRequest);
        Assert.Equal(HttpStatusCode.Created, portfolioResponse.StatusCode);

        var portfolio = await portfolioResponse.Content.ReadFromJsonAsync<Portfolio>();
        Assert.NotNull(portfolio);

        // 3. Create Account
        var createAccountRequest = new CreateAccountRequest
        {
            Name = "Test Stocks",
            AccountType = "Stocks",
            PortfolioId = portfolio.Id
        };

        var accountResponse = await _client.PostAsJsonAsync("/api/accounts", createAccountRequest);
        Assert.Equal(HttpStatusCode.Created, accountResponse.StatusCode);

        var account = await accountResponse.Content.ReadFromJsonAsync<Account>();
        Assert.NotNull(account);

        // 4. Create Buy Transaction
        var createTransactionRequest = new CreateTransactionRequest
        {
            AccountId = account.Id,
            TransactionType = "Buy",
            Symbol = "AAPL",
            Quantity = 10,
            Price = 150.00m
        };

        var transactionResponse = await _client.PostAsJsonAsync("/api/transactions", createTransactionRequest);
        Assert.Equal(HttpStatusCode.Created, transactionResponse.StatusCode);

        // 5. Verify Portfolio Summary
        var summaryResponse = await _client.GetAsync($"/api/portfolios/{portfolio.Id}/summary");
        Assert.Equal(HttpStatusCode.OK, summaryResponse.StatusCode);

        var summary = await summaryResponse.Content.ReadFromJsonAsync<PortfolioSummaryResponse>();
        Assert.NotNull(summary);
        Assert.Equal(1, summary.TotalHoldings);
        Assert.Equal(1500.00m, summary.TotalValue);
    }

    [Fact]
    public async Task GetPortfolio_ShouldReturn404_WhenNotFound()
    {
        var response = await _client.GetAsync("/api/portfolios/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatePortfolio_ShouldReturn400_WhenUserDoesNotExist()
    {
        var request = new CreatePortfolioRequest
        {
            Name = "Test",
            UserId = 99999
        };

        var response = await _client.PostAsJsonAsync("/api/portfolios", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
