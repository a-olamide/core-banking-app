using SharedKernel.Domain.Exceptions;
using SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.ValueObjects
{
    public sealed class AccountNumber : ValueObject
    {
        public string Value { get; }

        private AccountNumber(string value) => Value = value;

        public static AccountNumber Create(string value)
        {
            var normalized = (value ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new DomainException("ACCOUNT_NUMBER_REQUIRED", "Account number is required.");

            // Example rule: digits only, length 10 (adjust to your bank rules)
            if (normalized.Length != 10 || !normalized.All(char.IsDigit))
                throw new DomainException("ACCOUNT_NUMBER_INVALID", "Account number must be 10 digits.");

            return new AccountNumber(normalized);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
