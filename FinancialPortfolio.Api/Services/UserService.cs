namespace FinancialPortfolio.Api.Services;

using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs;


public class UserService : IUserService
{
    private readonly FinancialPortfolioDbContext _context;

    public UserService(FinancialPortfolioDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Portfolios)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Portfolios)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        var existingEmail = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (existingEmail)
            throw new ArgumentException("Email already exists");

        var user = new User
        {
            Username = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
