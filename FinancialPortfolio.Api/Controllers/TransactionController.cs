using Microsoft.AspNetCore.Mvc;
using FinancialPortfolio.Api.Services;
using FinancialPortfolio.Api.Models.DTOs;


namespace FinancialPortfolio.Api.Controllers;

public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ITransactionService transactionService,
        ILogger<TransactionController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    // GET: api/transactions/account/5
    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetAccountTransactions(int accountId)
    {
        try
        {
            var transactions = await _transactionService.GetAccountTransactionsAsync(accountId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for account {AccountId}", accountId);
            return StatusCode(500, new { message = "An error occurred while retrieving transactions" });
        }
    }

    // GET: api/transactions/portfolio/5
    [HttpGet("portfolio/{portfolioId}")]
    public async Task<IActionResult> GetPortfolioTransactions(int portfolioId)
    {
        try
        {
            var transactions = await _transactionService.GetPortfolioTransactionsAsync(portfolioId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for portfolio {PortfolioId}", portfolioId);
            return StatusCode(500, new { message = "An error occurred while retrieving transactions" });
        }
    }

    // GET: api/transactions/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(int id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound(new { message = $"Transaction with ID {id} not found" });
            }

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction {TransactionId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the transaction" });
        }
    }

    // POST: api/transactions
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        try
        {
            var transaction = await _transactionService.CreateTransactionAsync(request);
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(500, new { message = "An error occurred while creating the transaction" });
        }
    }

    // DELETE: api/transactions/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        try
        {
            var result = await _transactionService.DeleteTransactionAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Transaction with ID {id} not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction {TransactionId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the transaction" });
        }
    }
}
