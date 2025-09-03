using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.DTOs;
using MedicineAppApi.Services;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockMovementController : ControllerBase
    {
        private readonly IStockMovementService _stockMovementService;

        public StockMovementController(IStockMovementService stockMovementService)
        {
            _stockMovementService = stockMovementService;
        }

        // GET: api/stockmovement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovements()
        {
            var startDate = DateTime.UtcNow.AddDays(-30); // Last 30 days by default
            var endDate = DateTime.UtcNow;
            var stockMovements = await _stockMovementService.GetStockMovementsByDateRangeAsync(startDate, endDate);
            return Ok(stockMovements);
        }

        // GET: api/stockmovement/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockMovementDto>> GetStockMovement(int id)
        {
            var stockMovement = await _stockMovementService.GetStockMovementByIdAsync(id);
            if (stockMovement == null)
            {
                return NotFound();
            }
            return Ok(stockMovement);
        }

        // GET: api/stockmovement/medicine/5
        [HttpGet("medicine/{medicineId}")]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovementsByMedicine(int medicineId)
        {
            var stockMovements = await _stockMovementService.GetStockMovementsByMedicineAsync(medicineId);
            return Ok(stockMovements);
        }

        // GET: api/stockmovement/date-range?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovementsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var stockMovements = await _stockMovementService.GetStockMovementsByDateRangeAsync(startDate, endDate);
            return Ok(stockMovements);
        }

        // GET: api/stockmovement/reference/INVOICE/5
        [HttpGet("reference/{referenceType}/{referenceId}")]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovementsByReference(
            string referenceType, 
            int referenceId)
        {
            var stockMovements = await _stockMovementService.GetStockMovementsByReferenceAsync(referenceType, referenceId);
            return Ok(stockMovements);
        }

        // GET: api/stockmovement/user/5
        [HttpGet("user/{createdBy}")]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetStockMovementsByUser(int createdBy)
        {
            var stockMovements = await _stockMovementService.GetStockMovementsByCreatedByAsync(createdBy);
            return Ok(stockMovements);
        }

        // GET: api/stockmovement/summary/5
        [HttpGet("summary/{medicineId}")]
        public async Task<ActionResult<StockMovementSummaryDto>> GetStockMovementSummary(
            int medicineId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var summary = await _stockMovementService.GetStockMovementSummaryAsync(medicineId, startDate, endDate);
                return Ok(summary);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // GET: api/stockmovement/summaries
        [HttpGet("summaries")]
        public async Task<ActionResult<IEnumerable<StockMovementSummaryDto>>> GetAllStockMovementSummaries()
        {
            var summaries = await _stockMovementService.GetAllStockMovementSummariesAsync();
            return Ok(summaries);
        }

        // GET: api/stockmovement/inventory-value
        [HttpGet("inventory-value")]
        public async Task<ActionResult<object>> GetTotalInventoryValue()
        {
            var totalValue = await _stockMovementService.GetTotalInventoryValueAsync();
            return Ok(new { TotalInventoryValue = totalValue });
        }

        // GET: api/stockmovement/low-stock?threshold=10
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetLowStockMovements(
            [FromQuery] int threshold = 10)
        {
            var stockMovements = await _stockMovementService.GetLowStockMovementsAsync(threshold);
            return Ok(stockMovements);
        }

        // POST: api/stockmovement
        [HttpPost]
        public async Task<ActionResult<StockMovementDto>> CreateStockMovement(CreateStockMovementDto createStockMovementDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // TODO: Get actual user ID from JWT token
                // For now, using a default user ID
                int createdByUserId = 1;
                
                var stockMovement = await _stockMovementService.CreateStockMovementAsync(createStockMovementDto, createdByUserId);
                return CreatedAtAction(nameof(GetStockMovement), new { id = stockMovement.Id }, stockMovement);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/stockmovement/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockMovement(int id)
        {
            var result = await _stockMovementService.DeleteStockMovementAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
