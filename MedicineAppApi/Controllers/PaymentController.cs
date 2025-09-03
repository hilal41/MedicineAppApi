using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.DTOs;
using MedicineAppApi.Services;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments()
        {
            var startDate = DateTime.UtcNow.AddDays(-30); // Last 30 days by default
            var endDate = DateTime.UtcNow;
            var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
            return Ok(payments);
        }

        // GET: api/payment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        // GET: api/payment/invoice/5
        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByInvoice(int invoiceId)
        {
            var payments = await _paymentService.GetPaymentsByInvoiceAsync(invoiceId);
            return Ok(payments);
        }

        // GET: api/payment/method/cash
        [HttpGet("method/{method}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByMethod(string method)
        {
            var payments = await _paymentService.GetPaymentsByMethodAsync(method);
            return Ok(payments);
        }

        // GET: api/payment/date-range?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
            return Ok(payments);
        }

        // GET: api/payment/received-by/5
        [HttpGet("received-by/{receivedBy}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByReceivedBy(int receivedBy)
        {
            var payments = await _paymentService.GetPaymentsByReceivedByAsync(receivedBy);
            return Ok(payments);
        }

        // GET: api/payment/summary/invoice/5
        [HttpGet("summary/invoice/{invoiceId}")]
        public async Task<ActionResult<PaymentSummaryDto>> GetPaymentSummaryByInvoice(int invoiceId)
        {
            try
            {
                var summary = await _paymentService.GetPaymentSummaryByInvoiceAsync(invoiceId);
                return Ok(summary);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // GET: api/payment/analytics?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("analytics")]
        public async Task<ActionResult<object>> GetPaymentAnalytics(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var totalPayments = await _paymentService.GetTotalPaymentsByDateRangeAsync(startDate, endDate);
            var paymentCount = await _paymentService.GetPaymentCountByDateRangeAsync(startDate, endDate);

            // Get payments by method
            var cashPayments = await _paymentService.GetTotalPaymentsByMethodAsync("cash", startDate, endDate);
            var cardPayments = await _paymentService.GetTotalPaymentsByMethodAsync("card", startDate, endDate);
            var creditPayments = await _paymentService.GetTotalPaymentsByMethodAsync("credit", startDate, endDate);

            return Ok(new
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalPayments = totalPayments,
                PaymentCount = paymentCount,
                AveragePayment = paymentCount > 0 ? totalPayments / paymentCount : 0,
                PaymentsByMethod = new
                {
                    Cash = cashPayments,
                    Card = cardPayments,
                    Credit = creditPayments,
                    Other = totalPayments - cashPayments - cardPayments - creditPayments
                }
            });
        }

        // POST: api/payment
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment(CreatePaymentDto createPaymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // TODO: Get actual user ID from JWT token
                // For now, using a default user ID
                int receivedByUserId = 1;
                
                var payment = await _paymentService.CreatePaymentAsync(createPaymentDto, receivedByUserId);
                return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/payment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, UpdatePaymentDto updatePaymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var payment = await _paymentService.UpdatePaymentAsync(id, updatePaymentDto);
                return Ok(payment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/payment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
