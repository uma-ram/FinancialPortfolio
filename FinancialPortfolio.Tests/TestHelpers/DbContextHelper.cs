using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;

namespace FinancialPortfolio.Tests.TestHelpers;

public static class DbContextHelper
{
    public static FinancialPortfolioDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<FinancialPortfolioDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new FinancialPortfolioDbContext(options);
    }
}
