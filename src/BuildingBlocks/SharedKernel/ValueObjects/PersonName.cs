using SharedKernel.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.ValueObjects
{
    public sealed class PersonName : ValueObject
    {
        public string FirstName { get; }
        public string? MiddleName { get; }
        public string LastName { get; }

        private PersonName(string firstName, string? middleName, string lastName)
        {
            FirstName = Normalize(firstName);
            MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : Normalize(middleName);
            LastName = Normalize(lastName);

            if (string.IsNullOrWhiteSpace(FirstName))
                throw new ArgumentException("First name is required.", nameof(firstName));

            if (string.IsNullOrWhiteSpace(LastName))
                throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        public static PersonName Create(string firstName, string? middleName, string lastName)
            => new(firstName, middleName, lastName);

        private static string Normalize(string value) => value.Trim();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FirstName;
            yield return MiddleName;
            yield return LastName;
        }

        public override string ToString()
            => MiddleName is null
                ? $"{FirstName} {LastName}"
                : $"{FirstName} {MiddleName} {LastName}";
    }
}
