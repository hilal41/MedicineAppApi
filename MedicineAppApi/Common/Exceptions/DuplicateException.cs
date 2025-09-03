using System.Net;

namespace MedicineAppApi.Common.Exceptions
{
    public class DuplicateException : AppException
    {
        public DuplicateException(string message, string errorCode = "DUPLICATE_RESOURCE", object? additionalData = null)
            : base(message, HttpStatusCode.Conflict, errorCode, additionalData)
        {
        }

        public DuplicateException(string resourceName, string fieldName, object fieldValue, string errorCode = "DUPLICATE_RESOURCE")
            : base($"{resourceName} with {fieldName} '{fieldValue}' already exists.", HttpStatusCode.Conflict, errorCode, new { ResourceName = resourceName, FieldName = fieldName, FieldValue = fieldValue })
        {
        }
    }
}
