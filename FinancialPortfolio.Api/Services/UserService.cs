namespace FinancialPortfolio.Api.Services;

using AutoMapper;
using Azure;
using FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Models.DTOs.Requests;
using FinancialPortfolio.Api.Models.DTOs.Responses;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly FinancialPortfolioDbContext _context;
    private readonly IMapper _mapper;

    public UserService(FinancialPortfolioDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
       var response= await _context.Users
            .Include(u => u.Portfolios)
            .ToListAsync();
      return _mapper.Map<IEnumerable<UserResponse>>(response);
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var response = await _context.Users
            .Include(u => u.Portfolios)
            .FirstOrDefaultAsync(u => u.Id == id);
        return _mapper.Map<UserResponse>(response);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
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

        return _mapper.Map<UserResponse>(user);
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
