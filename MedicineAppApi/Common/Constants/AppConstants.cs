namespace MedicineAppApi.Common.Constants
{
    public static class AppConstants
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
            public const string Manager = "Manager";
        }

        public static class Status
        {
            public const string Active = "Active";
            public const string Inactive = "Inactive";
            public const string Pending = "Pending";
        }

        public static class Messages
        {
            public const string Success = "Operation completed successfully";
            public const string Error = "An error occurred";
            public const string NotFound = "Resource not found";
            public const string Unauthorized = "Unauthorized access";
            public const string InvalidCredentials = "Invalid email or password";
            public const string EmailExists = "User with this email already exists";
        }
    }
}
