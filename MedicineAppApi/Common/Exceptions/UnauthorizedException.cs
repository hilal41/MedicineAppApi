using System.Net;

namespace MedicineAppApi.Common.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized access", string errorCode = "UNAUTHORIZED", object? additionalData = null)
            : base(message, HttpStatusCode.Unauthorized, errorCode, additionalData)
        {
        }
    }
}
