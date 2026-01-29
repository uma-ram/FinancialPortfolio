namespace FinancialPortfolio.Tests.Services;

using Xunit;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;
using FinancialPortfolio.Api.Services;
using FinancialPortfolio.Tests.TestHelpers;
using Castle.Components.DictionaryAdapter.Xml;

public class PortfolioServiceTests
{
    [Fact]
    public async Task CreatePortfolioAsync_CreatePortfolioWhenUserExists()
    {
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new PortfolioService(context);

        //Arrange
        var user = new User();
        user.Username = "testuser";
        user.Email = "user@user.com";
        user.CreatedAt = DateTime.Now;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var request = new CreatePortfolioRequest()
        {
            Name = "UG",
            Description = "200k Portfolio",
            UserId = user.Id,
        };


        //Act
        var result = await service.CreatePortfolioAsync(request);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(result.Description, result.Description);
        Assert.Equal(user.Id, result.UserId);
    }


    [Fact]
    public async Task CreatePortfolioAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new PortfolioService(context);

        var request = new CreatePortfolioRequest
        {
            Name = "Test Portfolio",
            UserId = 999 // Non-existent user
        };

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            ()=>service.CreatePortfolioAsync(request));
    }

    [Fact]
    public async Task GetUserPortfolioAsync_ShouldReturnUserPortfolio()
    {
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new PortfolioService(context);

        //Arrange

        var user = new User { Username = "user", Email = "user@example.com", CreatedAt = DateTime.UtcNow };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var portfolio1 = new Portfolio { Name = "Portfolio 1", UserId = user.Id, CreatedAt = DateTime.UtcNow };
        var portfolio2 = new Portfolio { Name = "Portfolio 2", UserId = user.Id, CreatedAt = DateTime.UtcNow };
        context.Portfolios.AddRange(portfolio1, portfolio2);
        await context.SaveChangesAsync();
        
        //Act
        var result = await service.GetUserPortfoliosAsync(user.Id);

        // Assert
        Assert.Equal(2, result.Count());

    }
    [Fact]
    public async Task DeletePortfolioAsync_ShouldDeletePortfolio_WhenExists()
    {
        // Arrange
        var context = DbContextHelper.CreateInMemoryContext();
        var service = new PortfolioService(context);

        var user = new User { Username = "Test", Email = "test@example.com", CreatedAt = DateTime.UtcNow };
        context.Users.Add(user);
        var portfolio = new Portfolio { Name = "Test", UserId = user.Id, CreatedAt = DateTime.UtcNow };
        context.Portfolios.Add(portfolio);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeletePortfolioAsync(portfolio.Id);

        // Assert
        Assert.True(result);
        var deletedPortfolio = await context.Portfolios.FindAsync(portfolio.Id);
        Assert.Null(deletedPortfolio);
    }
}
