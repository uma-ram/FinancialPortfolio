using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialPortfolio.Api.Data;
using FinancialPortfolio.Api.Models;

namespace FinancialPortfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly FinancialPortfolioDbContext _context;

    public UserController(FinancialPortfolioDbContext context)
    {
        _context = context;
    }

    //GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _context.Users
            .Include(u => u.Portfolios)
            .ToListAsync();
        return Ok(users);
    }

    //Get: api/users/2
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Portfolios)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        return Ok(user);
    }

    //POST : api/users

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
    {
        var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingEmail != null)
        {
            return BadRequest(new { message = "Email Already exists !" });
        }
        var user = new User
        {
            Username = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}