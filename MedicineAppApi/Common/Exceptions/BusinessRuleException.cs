using System.Net;

namespace MedicineAppApi.Common.Exceptions
{
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION", object? additionalData = null)
            : base(message, HttpStatusCode.BadRequest, errorCode, additionalData)
        {
        }

        public BusinessRuleException(string ruleName, string message, string errorCode = "BUSINESS_RULE_VIOLATION")
            : base(message, HttpStatusCode.BadRequest, errorCode, new { RuleName = ruleName })
        {
        }
    }
}
