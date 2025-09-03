using System.Net;

namespace MedicineAppApi.Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message, string errorCode = "NOT_FOUND", object? additionalData = null)
            : base(message, HttpStatusCode.NotFound, errorCode, additionalData)
        {
        }

        public NotFoundException(string resourceName, object resourceId, string errorCode = "NOT_FOUND")
            : base($"{resourceName} with ID {resourceId} was not found.", HttpStatusCode.NotFound, errorCode, new { ResourceName = resourceName, ResourceId = resourceId })
        {
        }
    }
}
