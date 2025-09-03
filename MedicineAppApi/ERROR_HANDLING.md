# Global Error Handling System

## Overview

The Medicine Store Management System implements a comprehensive global error handling system that provides consistent, standardized error responses across all API endpoints. This system ensures that all exceptions are properly caught, logged, and returned in a uniform format.

## Architecture

### 1. Exception Hierarchy

```
Exception (Base)
└── AppException (Custom Base)
    ├── NotFoundException
    ├── ValidationException
    ├── UnauthorizedException
    ├── DuplicateException
    └── BusinessRuleException
```

### 2. Error Response Structure

All error responses follow this standardized format:

```json
{
  "message": "Error description",
  "errorCode": "ERROR_CODE",
  "statusCode": 400,
  "timestamp": "2024-01-01T00:00:00.000Z",
  "traceId": "unique-trace-id",
  "errors": ["Error 1", "Error 2"],
  "additionalData": { /* optional additional context */ }
}
```

## Exception Types

### 1. AppException (Base)
- **Purpose**: Base class for all custom exceptions
- **Properties**: StatusCode, ErrorCode, AdditionalData
- **Usage**: Extended by specific exception types

### 2. NotFoundException
- **HTTP Status**: 404 Not Found
- **Usage**: When a requested resource doesn't exist
- **Example**: Medicine, User, Invoice not found

```csharp
throw new NotFoundException("Medicine", medicineId, "MEDICINE_NOT_FOUND");
```

### 3. ValidationException
- **HTTP Status**: 400 Bad Request
- **Usage**: When input validation fails
- **Properties**: Errors (List<string>)
- **Example**: Invalid email format, required fields missing

```csharp
var errors = new List<string> { "Email is required", "Password too short" };
throw new ValidationException("Validation failed", errors, "VALIDATION_ERROR");
```

### 4. UnauthorizedException
- **HTTP Status**: 401 Unauthorized
- **Usage**: When user authentication/authorization fails
- **Example**: Invalid credentials, expired token

```csharp
throw new UnauthorizedException("Invalid credentials", "INVALID_CREDENTIALS");
```

### 5. DuplicateException
- **HTTP Status**: 409 Conflict
- **Usage**: When trying to create a resource that already exists
- **Example**: Duplicate email, duplicate medicine name

```csharp
throw new DuplicateException("User", "email", email, "USER_ALREADY_EXISTS");
```

### 6. BusinessRuleException
- **HTTP Status**: 400 Bad Request
- **Usage**: When business logic rules are violated
- **Example**: Insufficient stock, payment exceeds balance

```csharp
throw new BusinessRuleException("Insufficient stock", "INSUFFICIENT_STOCK", 
    new { AvailableStock = 5, RequestedQuantity = 10 });
```

## Middleware Components

### 1. GlobalExceptionHandler
- **Purpose**: Catches all unhandled exceptions
- **Location**: `Common/Middleware/GlobalExceptionHandler.cs`
- **Features**:
  - Exception type-specific handling
  - Structured error responses
  - Logging with trace IDs
  - Development vs Production error details

### 2. ValidationErrorHandler
- **Purpose**: Handles model validation errors
- **Location**: `Common/Middleware/ValidationErrorHandler.cs`
- **Features**:
  - Standardizes validation error format
  - Extracts field-specific errors
  - Maintains consistent response structure

## Usage Examples

### In Controllers

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<MedicineDto>> GetMedicine(int id)
{
    var medicine = await _medicineRepository.GetByIdAsync(id);
    if (medicine == null)
    {
        throw new NotFoundException("Medicine", id, "MEDICINE_NOT_FOUND");
    }
    return Ok(_mapper.Map<MedicineDto>(medicine));
}
```

### In Services

```csharp
public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto, int userId)
{
    var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId);
    if (invoice == null)
    {
        throw new NotFoundException("Invoice", dto.InvoiceId, "INVOICE_NOT_FOUND");
    }

    if (dto.Amount > remainingBalance)
    {
        throw new BusinessRuleException("Payment amount exceeds remaining balance", 
            "PAYMENT_AMOUNT_EXCEEDS_BALANCE", 
            new { InvoiceNo = invoice.InvoiceNo, PaymentAmount = dto.Amount, RemainingBalance = remainingBalance });
    }

    // Continue with payment creation...
}
```

### In Repositories

```csharp
public async Task<Medicine> GetByIdAsync(int id)
{
    var medicine = await _dbSet.FindAsync(id);
    if (medicine == null)
    {
        throw new NotFoundException("Medicine", id, "MEDICINE_NOT_FOUND");
    }
    return medicine;
}
```

## Error Response Examples

### 1. Not Found Error
```json
{
  "message": "Medicine with ID 123 was not found.",
  "errorCode": "MEDICINE_NOT_FOUND",
  "statusCode": 404,
  "timestamp": "2024-01-01T00:00:00.000Z",
  "traceId": "0HMQ8V9K2QJ3P:00000001",
  "errors": [],
  "additionalData": {
    "resourceName": "Medicine",
    "resourceId": 123
  }
}
```

### 2. Validation Error
```json
{
  "message": "One or more validation errors occurred.",
  "errorCode": "VALIDATION_ERROR",
  "statusCode": 400,
  "timestamp": "2024-01-01T00:00:00.000Z",
  "traceId": "0HMQ8V9K2QJ3P:00000002",
  "errors": [
    "Email is required",
    "Password must be at least 8 characters",
    "Age must be between 18 and 100"
  ],
  "additionalData": null
}
```

### 3. Business Rule Error
```json
{
  "message": "Insufficient stock",
  "errorCode": "INSUFFICIENT_STOCK",
  "statusCode": 400,
  "timestamp": "2024-01-01T00:00:00.000Z",
  "traceId": "0HMQ8V9K2QJ3P:00000003",
  "errors": [],
  "additionalData": {
    "availableStock": 5,
    "requestedQuantity": 10
  }
}
```

### 4. Unauthorized Error
```json
{
  "message": "Invalid email or password",
  "errorCode": "INVALID_CREDENTIALS",
  "statusCode": 401,
  "timestamp": "2024-01-01T00:00:00.000Z",
  "traceId": "0HMQ8V9K2QJ3P:00000004",
  "errors": [],
  "additionalData": null
}
```

## Testing Error Handling

Use the TestErrorController to test different error scenarios:

- `GET /api/testerror/not-found` - Test NotFoundException
- `GET /api/testerror/validation` - Test ValidationException
- `GET /api/testerror/unauthorized` - Test UnauthorizedException
- `GET /api/testerror/duplicate` - Test DuplicateException
- `GET /api/testerror/business-rule` - Test BusinessRuleException
- `GET /api/testerror/argument` - Test ArgumentException
- `GET /api/testerror/unexpected` - Test unexpected exceptions

## Best Practices

### 1. Exception Selection
- Use specific exception types for different scenarios
- Provide meaningful error codes for client handling
- Include relevant additional data for debugging

### 2. Error Messages
- Keep messages user-friendly in production
- Include technical details in development
- Use consistent message formatting

### 3. Logging
- All exceptions are automatically logged with trace IDs
- Use trace IDs for debugging and support
- Log sensitive information carefully

### 4. Client Handling
- Clients should check the `errorCode` for specific handling
- Use `traceId` for support requests
- Handle different status codes appropriately

## Configuration

The error handling system is configured in `Program.cs`:

```csharp
// Add global exception handling middleware (must be early in pipeline)
app.UseGlobalExceptionHandler();

// Add validation error handling middleware
app.UseValidationErrorHandler();
```

## Security Considerations

- Sensitive information is not exposed in error messages
- Stack traces are only shown in development
- Error codes don't reveal internal system details
- All exceptions are logged for monitoring

## Monitoring and Debugging

- All exceptions include trace IDs for correlation
- Error responses include timestamps for tracking
- Additional data provides context for debugging
- Logs include exception details and stack traces
