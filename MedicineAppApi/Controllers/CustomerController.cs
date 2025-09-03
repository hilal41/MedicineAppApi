using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.DTOs;
using MedicineAppApi.Services;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _customerService.GetTopCustomersAsync(50); // Get top 50 customers
            return Ok(customers);
        }

        // GET: api/customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // GET: api/customer/name/John Doe
        [HttpGet("name/{name}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerByName(string name)
        {
            var customer = await _customerService.GetCustomerByNameAsync(name);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // GET: api/customer/phone/1234567890
        [HttpGet("phone/{phone}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerByPhone(string phone)
        {
            var customer = await _customerService.GetCustomerByPhoneAsync(phone);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // GET: api/customer/search?term=john
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> SearchCustomers([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term is required");
            }

            var customers = await _customerService.SearchCustomersAsync(term);
            return Ok(customers);
        }

        // GET: api/customer/top?count=10
        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetTopCustomers([FromQuery] int count = 10)
        {
            var customers = await _customerService.GetTopCustomersAsync(count);
            return Ok(customers);
        }

        // GET: api/customer/5/total-spent
        [HttpGet("{id}/total-spent")]
        public async Task<ActionResult<object>> GetCustomerTotalSpent(int id)
        {
            var totalSpent = await _customerService.GetTotalSpentByCustomerAsync(id);
            var invoiceCount = await _customerService.GetInvoiceCountByCustomerAsync(id);

            return Ok(new
            {
                CustomerId = id,
                TotalSpent = totalSpent,
                InvoiceCount = invoiceCount,
                AverageSpent = invoiceCount > 0 ? totalSpent / invoiceCount : 0
            });
        }

        // POST: api/customer
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto createCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customer = await _customerService.CreateCustomerAsync(createCustomerDto);
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto updateCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customer = await _customerService.UpdateCustomerAsync(id, updateCustomerDto);
                return Ok(customer);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
