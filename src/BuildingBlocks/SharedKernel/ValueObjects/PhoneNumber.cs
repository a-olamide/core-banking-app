using SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.ValueObjects
{
    public sealed class PhoneNumber : ValueObject
    {
        public string CountryCode { get; }
        public string Number { get; }

        private PhoneNumber(string countryCode, string number)
        {
            CountryCode = (countryCode ?? string.Empty).Trim();
            Number = (number ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(CountryCode))
                throw new ArgumentException("Country code is required.", nameof(countryCode));

            if (string.IsNullOrWhiteSpace(Number))
                throw new ArgumentException("Phone number is required.", nameof(number));
            //You can add other validations for phoneNo
        }

        public static PhoneNumber Create(string countryCode, string number)
            => new(countryCode, number);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return CountryCode;
            yield return Number;
        }

        public override string ToString() => $"{CountryCode}{Number}";
    }
}
