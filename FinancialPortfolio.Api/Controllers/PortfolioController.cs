using Microsoft.AspNetCore.Mvc;
using FinancialPortfolio.Api.Services;
using FinancialPortfolio.Api.Models.DTOs;

namespace FinancialPortfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioService _portfolioService;
    private readonly ILogger<PortfolioController> _logger;

    public PortfolioController(IPortfolioService portfolioService, ILogger<PortfolioController> logger  )
    {
        _portfolioService = portfolioService;
        _logger = logger;
    }

    // GET: api/portfolios/user/5
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPortfolios(int userId)
    {
        try
        {
            var portfolios = await _portfolioService.GetUserPortfoliosAsync(userId);
            return Ok(portfolios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving portfolios for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving portfolios" });
        }
    }

    // GET: api/portfolios/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPortfolio(int id)
    {
        try
        {
            var portfolio = await _portfolioService.GetPortfolioByIdAsync(id);

            if (portfolio == null)
            {
                return NotFound(new { message = $"Portfolio with ID {id} not found" });
            }

            return Ok(portfolio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving portfolio {PortfolioId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the portfolio" });
        }
    }

    // GET: api/portfolios/5/summary
    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetPortfolioSummary(int id)
    {
        try
        {
            var summary = await _portfolioService.GetPortfolioSummaryAsync(id);

            if (summary == null)
            {
                return NotFound(new { message = $"Portfolio with ID {id} not found" });
            }

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving portfolio summary {PortfolioId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the portfolio summary" });
        }
    }

    // POST: api/portfolios
    [HttpPost]
    public async Task<IActionResult> CreatePortfolio([FromBody] CreatePortfolioRequest request)
    {
        try
        {
            var portfolio = await _portfolioService.CreatePortfolioAsync(request);
            return CreatedAtAction(nameof(GetPortfolio), new { id = portfolio.Id }, portfolio);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating portfolio");
            return StatusCode(500, new { message = "An error occurred while creating the portfolio" });
        }
    }

    // PUT: api/portfolios/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePortfolio(int id, [FromBody] CreatePortfolioRequest request)
    {
        try
        {
            var portfolio = await _portfolioService.UpdatePortfolioAsync(id, request);

            if (portfolio == null)
            {
                return NotFound(new { message = $"Portfolio with ID {id} not found" });
            }

            return Ok(portfolio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating portfolio {PortfolioId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the portfolio" });
        }
    }

    // DELETE: api/portfolios/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePortfolio(int id)
    {
        try
        {
            var result = await _portfolioService.DeletePortfolioAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Portfolio with ID {id} not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting portfolio {PortfolioId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the portfolio" });
        }
    }

}
