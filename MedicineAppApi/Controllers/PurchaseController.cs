using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.DTOs;
using MedicineAppApi.Services;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        // GET: api/purchase
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetPurchases()
        {
            var startDate = DateTime.UtcNow.AddDays(-30); // Last 30 days by default
            var endDate = DateTime.UtcNow;
            var purchases = await _purchaseService.GetPurchasesByDateRangeAsync(startDate, endDate);
            return Ok(purchases);
        }

        // GET: api/purchase/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDto>> GetPurchase(int id)
        {
            var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            return Ok(purchase);
        }

        // GET: api/purchase/invoiceno/PUR-001
        [HttpGet("invoiceno/{invoiceNo}")]
        public async Task<ActionResult<PurchaseDto>> GetPurchaseByInvoiceNo(string invoiceNo)
        {
            var purchase = await _purchaseService.GetPurchaseByInvoiceNoAsync(invoiceNo);
            if (purchase == null)
            {
                return NotFound();
            }
            return Ok(purchase);
        }

        // GET: api/purchase/supplier/5
        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetPurchasesBySupplier(int supplierId)
        {
            var purchases = await _purchaseService.GetPurchasesBySupplierAsync(supplierId);
            return Ok(purchases);
        }

        // GET: api/purchase/date-range?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetPurchasesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var purchases = await _purchaseService.GetPurchasesByDateRangeAsync(startDate, endDate);
            return Ok(purchases);
        }

        // GET: api/purchase/user/5
        [HttpGet("user/{createdBy}")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetPurchasesByUser(int createdBy)
        {
            var purchases = await _purchaseService.GetPurchasesByCreatedByAsync(createdBy);
            return Ok(purchases);
        }

        // GET: api/purchase/summary?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetPurchaseSummary(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var totalPurchases = await _purchaseService.GetTotalPurchasesAsync(startDate, endDate);
            var purchaseCount = await _purchaseService.GetPurchaseCountAsync(startDate, endDate);

            return Ok(new
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalPurchases = totalPurchases,
                PurchaseCount = purchaseCount,
                AveragePurchase = purchaseCount > 0 ? totalPurchases / purchaseCount : 0
            });
        }

        // POST: api/purchase
        [HttpPost]
        public async Task<ActionResult<PurchaseDto>> CreatePurchase(CreatePurchaseDto createPurchaseDto)
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
                
                var purchase = await _purchaseService.CreatePurchaseAsync(createPurchaseDto, createdByUserId);
                return CreatedAtAction(nameof(GetPurchase), new { id = purchase.Id }, purchase);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/purchase/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, UpdatePurchaseDto updatePurchaseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var purchase = await _purchaseService.UpdatePurchaseAsync(id, updatePurchaseDto);
                return Ok(purchase);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/purchase/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var result = await _purchaseService.DeletePurchaseAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
