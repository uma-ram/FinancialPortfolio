using FinancialPortfolio.Api.Models.DTOs.Responses;
using FinancialPortfolio.Api.Services;
using Microsoft.AspNetCore.Mvc;


namespace FinancialPortfolio.Api.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }
    // GET: api/analytics/portfolio/5
    [HttpGet("portfolio/{portfolioId}")]
    public async Task<ActionResult<PortfolioAnalyticsResponse>> GetPortfolioAnalytics(int portfolioId)
    {
        try
        {
            var analytics = await _analyticsService.GetPortfolioAnalyticsAsync(portfolioId);

            if (analytics == null)
            {
                return NotFound(new { message = $"Portfolio with ID {portfolioId} not found" });
            }

            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving portfolio analytics for {PortfolioId}", portfolioId);
            return StatusCode(500, new { message = "An error occurred while retrieving analytics" });
        }
    }

    // GET: api/analytics/portfolio/5/history
    [HttpGet("portfolio/{portfolioId}/history")]
    public async Task<ActionResult<TransactionHistoryResponse>> GetTransactionHistory(
        int portfolioId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? transactionType = null)
    {
        try
        {
            var history = await _analyticsService.GetTransactionHistoryAsync(
                portfolioId, startDate, endDate, transactionType);

            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction history for {PortfolioId}", portfolioId);
            return StatusCode(500, new { message = "An error occurred while retrieving transaction history" });
        }
    }
}
