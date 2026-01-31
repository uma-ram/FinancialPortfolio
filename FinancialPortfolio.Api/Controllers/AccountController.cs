using Microsoft.AspNetCore.Mvc;
using FinancialPortfolio.Api.Services;
using FinancialPortfolio.Api.Models.DTOs;

namespace FinancialPortfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAccountService accountService,
        ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    // GET: api/accounts/portfolio/5
    [HttpGet("portfolio/{portfolioId}")]
    public async Task<IActionResult> GetPortfolioAccounts(int portfolioId)
    {
        try
        {
            var accounts = await _accountService.GetPortfolioAccountsAsync(portfolioId);
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts for portfolio {PortfolioId}", portfolioId);
            return StatusCode(500, new { message = "An error occurred while retrieving accounts" });
        }
    }

    // GET: api/accounts/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);

            if (account == null)
            {
                return NotFound(new { message = $"Account with ID {id} not found" });
            }

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account {AccountId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the account" });
        }
    }

    // POST: api/accounts
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            var account = await _accountService.CreateAccountAsync(request);
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return StatusCode(500, new { message = "An error occurred while creating the account" });
        }
    }

    // DELETE: api/accounts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        try
        {
            var result = await _accountService.DeleteAccountAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Account with ID {id} not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account {AccountId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the account" });
        }
    }
}
