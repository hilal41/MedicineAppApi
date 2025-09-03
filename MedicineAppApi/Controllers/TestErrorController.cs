using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.Common.Exceptions;
using MedicineAppApi.Common.Models;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestErrorController : ControllerBase
    {
        [HttpGet("not-found")]
        public IActionResult TestNotFound()
        {
            throw new NotFoundException("Test Resource", 123, "TEST_NOT_FOUND");
        }

        [HttpGet("validation")]
        public IActionResult TestValidation()
        {
            var errors = new List<string>
            {
                "Email is required",
                "Password must be at least 8 characters",
                "Age must be between 18 and 100"
            };
            throw new ValidationException("Validation failed", errors, "TEST_VALIDATION");
        }

        [HttpGet("unauthorized")]
        public IActionResult TestUnauthorized()
        {
            throw new UnauthorizedException("You don't have permission to access this resource", "TEST_UNAUTHORIZED");
        }

        [HttpGet("duplicate")]
        public IActionResult TestDuplicate()
        {
            throw new DuplicateException("User", "email", "test@example.com", "TEST_DUPLICATE");
        }

        [HttpGet("business-rule")]
        public IActionResult TestBusinessRule()
        {
            throw new BusinessRuleException("Insufficient stock", "INSUFFICIENT_STOCK", new { AvailableStock = 5, RequestedQuantity = 10 });
        }

        [HttpGet("argument")]
        public IActionResult TestArgument([FromQuery] string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value parameter is required");
            }
            return Ok(new { message = "Success", value });
        }

        [HttpGet("null-argument")]
        public IActionResult TestNullArgument([FromQuery] string? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return Ok(new { message = "Success", value });
        }

        [HttpGet("invalid-operation")]
        public IActionResult TestInvalidOperation()
        {
            throw new InvalidOperationException("This operation is not allowed in the current state");
        }

        [HttpGet("key-not-found")]
        public IActionResult TestKeyNotFound()
        {
            throw new KeyNotFoundException("The specified key was not found");
        }

        [HttpGet("unexpected")]
        public IActionResult TestUnexpected()
        {
            throw new Exception("This is an unexpected error");
        }

        [HttpGet("divide-by-zero")]
        public IActionResult TestDivideByZero()
        {
            var result = 10 / 0;
            return Ok(result);
        }

        [HttpGet("model-validation")]
        public IActionResult TestModelValidation([FromQuery] string email, [FromQuery] int age)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(new { email, age });
        }
    }
}
