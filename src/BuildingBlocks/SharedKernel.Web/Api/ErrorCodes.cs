using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Web.Api
{
    public static class ErrorCodes
    {
        // Generic
        public const string ValidationError = "VALIDATION_ERROR";
        public const string NotFound = "NOT_FOUND";
        public const string Conflict = "CONFLICT";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";

        // Domain examples (you’ll add more as you implement)
        public const string InsufficientFunds = "INSUFFICIENT_FUNDS";
        public const string AccountFrozen = "ACCOUNT_FROZEN";
        public const string CustomerEmailAlreadyExists = "CUSTOMER_EMAIL_EXISTS";
    }
}
