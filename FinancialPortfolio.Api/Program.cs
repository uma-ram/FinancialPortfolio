using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using FinancialPortfolio.Api.Data;

var builder = WebApplication.CreateBuilder(args);

//Add dbcontent

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<FinancialPortfolioDbContext>(options =>
        options.UseInMemoryDatabase("FinancialPortfolioDB"));
}
else
{
    builder.Services.AddDbContext<FinancialPortfolioDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Basic health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck");

app.Run();

// Make Program class public for testing
public partial class Program { }
