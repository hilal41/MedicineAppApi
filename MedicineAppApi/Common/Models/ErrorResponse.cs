using System.Net;

namespace MedicineAppApi.Common.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public object? AdditionalData { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string TraceId { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();

        public ErrorResponse()
        {
        }

        public ErrorResponse(string message, string errorCode, HttpStatusCode statusCode, object? additionalData = null)
        {
            Message = message;
            ErrorCode = errorCode;
            StatusCode = statusCode;
            AdditionalData = additionalData;
        }

        public ErrorResponse(string message, string errorCode, HttpStatusCode statusCode, List<string> errors, object? additionalData = null)
        {
            Message = message;
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Errors = errors;
            AdditionalData = additionalData;
        }
    }

    public class ValidationErrorResponse : ErrorResponse
    {
        public Dictionary<string, List<string>> FieldErrors { get; set; } = new Dictionary<string, List<string>>();

        public ValidationErrorResponse(string message, Dictionary<string, List<string>> fieldErrors)
            : base(message, "VALIDATION_ERROR", HttpStatusCode.BadRequest)
        {
            FieldErrors = fieldErrors;
            Errors = fieldErrors.Values.SelectMany(v => v).ToList();
        }
    }
}
