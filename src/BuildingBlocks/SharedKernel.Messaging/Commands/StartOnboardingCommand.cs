using SharedKernel.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Messaging.Commands
{
    public sealed record StartOnboardingCommand(
    Guid CorrelationId,
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
    string CountryCode,
    string Currency,
    int AccountType
) : ICorrelatedMessage;
}
