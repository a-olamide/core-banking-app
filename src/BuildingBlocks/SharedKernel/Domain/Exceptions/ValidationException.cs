using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domain.Exceptions
{
    public sealed class ValidationException : DomainException
    {
        public IDictionary<string, string[]> Details { get; }

        public ValidationException(
            string errorCode,
            string? message,
            IDictionary<string, string[]> details)
            : base(errorCode, message)
        {
            Details = details;
        }
    }
}
