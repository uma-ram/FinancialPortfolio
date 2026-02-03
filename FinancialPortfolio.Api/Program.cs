using FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using AutoMapper;
using FinancialPortfolio.Api.Mappings;
using Microsoft.Extensions.DependencyInjection;

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
// Register Services (Dependency Injection)
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPriceUpdateService, PriceUpdateService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers
builder.Services.AddControllers();

//Automapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());


var app = builder.Build();

// Configure the HTTP request pipeline.


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

////Basic health check endpoint
//app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
//.WithName("HealthCheck");

app.MapControllers();

app.Run();


// Make Program class public for testing
public partial class Program { }
