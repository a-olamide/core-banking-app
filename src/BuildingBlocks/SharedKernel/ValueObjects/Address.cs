using SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.ValueObjects
{
    public sealed class Address : ValueObject
    {
        public string Line1 { get; }
        public string? Line2 { get; }
        public string City { get; }
        public string StateOrProvince { get; }
        public string PostalCode { get; }
        public string CountryCode { get; }

        private Address(
            string line1,
            string? line2,
            string city,
            string stateOrProvince,
            string postalCode,
            string countryCode)
        {
            Line1 = Normalize(line1);
            Line2 = string.IsNullOrWhiteSpace(line2) ? null : Normalize(line2);
            City = Normalize(city);
            StateOrProvince = Normalize(stateOrProvince);
            PostalCode = Normalize(postalCode);
            CountryCode = Normalize(countryCode).ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(Line1))
                throw new ArgumentException("Address line1 is required.", nameof(line1));
            if (string.IsNullOrWhiteSpace(City))
                throw new ArgumentException("City is required.", nameof(city));
            if (string.IsNullOrWhiteSpace(CountryCode))
                throw new ArgumentException("Country code is required.", nameof(countryCode));
        }

        public static Address Create(
            string line1,
            string? line2,
            string city,
            string stateOrProvince,
            string postalCode,
            string countryCode)
            => new(line1, line2, city, stateOrProvince, postalCode, countryCode);

        private static string Normalize(string value) => (value ?? string.Empty).Trim();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Line1;
            yield return Line2;
            yield return City;
            yield return StateOrProvince;
            yield return PostalCode;
            yield return CountryCode;
        }
    }
}
