namespace MedicineAppApi.Common.Exceptions
{
    public class AppException : Exception
    {
        public AppException() : base() { }
        public AppException(string message) : base(message) { }
        public AppException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message = "Resource not found") : base(message) { }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized access") : base(message) { }
    }

    public class ValidationException : AppException
    {
        public ValidationException(string message = "Validation failed") : base(message) { }
    }

    public class DuplicateException : AppException
    {
        public DuplicateException(string message = "Resource already exists") : base(message) { }
    }
}
