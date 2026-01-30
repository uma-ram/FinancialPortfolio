using FinancialPortfolio.Api.Models.DTOs;
using FinancialPortfolio.Api.Models;
using FinancialPortfolio.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialPortfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    //GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        _logger.LogInformation("Fetching all users");
        var users = await _userService.GetAllUsersAsync();
        _logger.LogInformation("Fetched {Count} users", users.Count());
        return Ok(users);
    }

    //Get: api/users/2
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        _logger.LogInformation("Fetching user with ID {UserId}", id);

        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found", id);
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        _logger.LogInformation("User with ID {UserId} fetched successfully", id);

        return Ok(user);
    }

    //POST : api/users

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
    {
        _logger.LogInformation("Creating user with email {Email}", request.Email);

        try
        {
            var user = await _userService.CreateUserAsync(request);

            _logger.LogInformation("User created successfully with ID {UserId}", user.Id);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "User creation failed for email {Email}", request.Email);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating user with email {Email}", request.Email);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        _logger.LogInformation("Deleting user with ID {UserId}", id);

        var deleted = await _userService.DeleteUserAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed. User with ID {UserId} not found", id);
            return NotFound(new { message = $"User with ID {id} not found" });
        }

        _logger.LogInformation("User with ID {UserId} deleted successfully", id);

        return NoContent();
    }
}