namespace FinancialPortfolio.Api.Services;

using FinancialPortfolio.Api.Models.DTOs;
using FinancialPortfolio.Api.Models;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<bool> DeleteUserAsync(int id);
}
