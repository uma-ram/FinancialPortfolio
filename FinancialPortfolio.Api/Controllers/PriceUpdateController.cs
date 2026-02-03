using FinancialPortfolio.Api.Services;
using Microsoft.AspNetCore.Mvc;
using FinancialPortfolio.Api.Models.DTOs.Requests;

namespace FinancialPortfolio.Api.Controllers;

[ApiController]
[Route("api/priceupdate")]
public class PriceUpdateController : ControllerBase
{
    private readonly IPriceUpdateService _priceUpdateService;
    private readonly ILogger<PriceUpdateController> _logger;

    public PriceUpdateController(IPriceUpdateService priceUpdateService,
        ILogger<PriceUpdateController> logger)
    {
        _priceUpdateService = priceUpdateService;
        _logger = logger;
    }

    // PUT: api/priceupdates/holding/5
    [HttpPut("holding/{holdingId}")]
    public async Task<IActionResult> UpdateHoldingPrice(int holdingId, [FromBody] UpdatePriceRequest request)
    {
        try
        {
            await _priceUpdateService.UpdateHoldingPriceAsync(holdingId, request.NewPrice);
            return Ok(new { message = "Price updated successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating holding price");
            return StatusCode(500, new { message = "An error occurred while updating the price" });
        }
    }
    // PUT: api/priceupdates/portfolio/5
    [HttpPut("portfolio/{portfolioId}")]
    public async Task<IActionResult> UpdatePortfolioPrices(int portfolioId, [FromBody] UpdatePortfolioPricesRequest request)
    {
        try
        {
            await _priceUpdateService.UpdatePortfolioHoldingPricesAsync(portfolioId, request.SymbolPrices);
            return Ok(new { message = "Portfolio prices updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating portfolio prices");
            return StatusCode(500, new { message = "An error occurred while updating prices" });
        }
    }

    // POST: api/priceupdates/fetch
    [HttpPost("fetch")]
    public async Task<IActionResult> FetchCurrentPrices([FromBody] FetchPricesRequest request)
    {
        try
        {
            var prices = await _priceUpdateService.GetCurrentPricesAsync(request.Symbols);
            return Ok(prices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching prices");
            return StatusCode(500, new { message = "An error occurred while fetching prices" });
        }
    }
}
