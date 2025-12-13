using SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("Email is required.", nameof(value));

            // intentionally simple; can be replaced with stricter validation later
            if (!normalized.Contains('@'))
                throw new ArgumentException("Invalid email format.", nameof(value));

            Value = normalized;
        }

        public static Email Create(string value) => new(value);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
