using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.DTOs;
using MedicineAppApi.Services;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // GET: api/invoice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoices()
        {
            var startDate = DateTime.UtcNow.AddDays(-30); // Last 30 days by default
            var endDate = DateTime.UtcNow;
            var invoices = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);
            return Ok(invoices);
        }

        // GET: api/invoice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        // GET: api/invoice/invoiceno/INV-001
        [HttpGet("invoiceno/{invoiceNo}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceByInvoiceNo(string invoiceNo)
        {
            var invoice = await _invoiceService.GetInvoiceByInvoiceNoAsync(invoiceNo);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }

        // GET: api/invoice/date-range?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoicesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var invoices = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);
            return Ok(invoices);
        }

        // GET: api/invoice/customer/5
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoicesByCustomer(int customerId)
        {
            var invoices = await _invoiceService.GetInvoicesByCustomerAsync(customerId);
            return Ok(invoices);
        }

        // GET: api/invoice/sales-summary?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("sales-summary")]
        public async Task<ActionResult<object>> GetSalesSummary(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var totalSales = await _invoiceService.GetTotalSalesAsync(startDate, endDate);
            var invoiceCount = await _invoiceService.GetInvoiceCountAsync(startDate, endDate);

            return Ok(new
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = totalSales,
                InvoiceCount = invoiceCount,
                AverageSale = invoiceCount > 0 ? totalSales / invoiceCount : 0
            });
        }

        // POST: api/invoice
        [HttpPost]
        public async Task<ActionResult<InvoiceDto>> CreateInvoice(CreateInvoiceDto createInvoiceDto)
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
                
                var invoice = await _invoiceService.CreateInvoiceAsync(createInvoiceDto, createdByUserId);
                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/invoice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, UpdateInvoiceDto updateInvoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var invoice = await _invoiceService.UpdateInvoiceAsync(id, updateInvoiceDto);
                return Ok(invoice);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/invoice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _invoiceService.DeleteInvoiceAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
