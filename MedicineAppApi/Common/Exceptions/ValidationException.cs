using System.Net;

namespace MedicineAppApi.Common.Exceptions
{
    public class ValidationException : AppException
    {
        public List<string> Errors { get; set; }

        public ValidationException(string message, List<string> errors, string errorCode = "VALIDATION_ERROR")
            : base(message, HttpStatusCode.BadRequest, errorCode, errors)
        {
            Errors = errors;
        }

        public ValidationException(string message, string errorCode = "VALIDATION_ERROR")
            : base(message, HttpStatusCode.BadRequest, errorCode)
        {
            Errors = new List<string> { message };
        }

        public ValidationException(List<string> errors, string errorCode = "VALIDATION_ERROR")
            : base("One or more validation errors occurred.", HttpStatusCode.BadRequest, errorCode, errors)
        {
            Errors = errors;
        }
    }
}
