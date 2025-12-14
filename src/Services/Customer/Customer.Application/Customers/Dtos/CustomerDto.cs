using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Customers.Dtos
{
    public sealed record CustomerDto(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string PhoneCountryCode,
    string PhoneNumber,
    string CountryCode,
    string City
);
}
