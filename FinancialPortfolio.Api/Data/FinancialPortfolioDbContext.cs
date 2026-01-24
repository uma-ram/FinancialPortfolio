

using FinancialPortfolio.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace FinancialPortfolio.Api.Data;

public class FinancialPortfolioDbContext : DbContext
{
    public FinancialPortfolioDbContext(DbContextOptions<FinancialPortfolioDbContext> options) :
        base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Holding> Holdings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        // Portfolio configuration
        modelBuilder.Entity<Portfolio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(e => e.User)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Portfolio)
                .WithMany(p => p.Accounts)
                .HasForeignKey(e => e.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Symbol);
            entity.HasIndex(e => e.TransactionDate);
        });

        // Holding configuration
        modelBuilder.Entity<Holding>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Portfolio)
                .WithMany(p => p.Holdings)
                .HasForeignKey(e => e.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.PortfolioId, e.Symbol }).IsUnique();
        });
    }

}
