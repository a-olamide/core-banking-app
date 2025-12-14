using Customer.Application.Customers.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Customers.Command.CreateCustomer
{
    public sealed record CreateCustomerCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    string PhoneCountryCode,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string StateOrProvince,
    string PostalCode,
    string CountryCode
) : IRequest<CustomerDto>;
}
