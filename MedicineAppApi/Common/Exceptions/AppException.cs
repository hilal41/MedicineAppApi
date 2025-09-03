using System.Net;

namespace MedicineAppApi.Common.Exceptions
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public object? AdditionalData { get; set; }

        public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string errorCode = "GENERAL_ERROR", object? additionalData = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            AdditionalData = additionalData;
        }

        public AppException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string errorCode = "GENERAL_ERROR", object? additionalData = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            AdditionalData = additionalData;
        }
    }
}
