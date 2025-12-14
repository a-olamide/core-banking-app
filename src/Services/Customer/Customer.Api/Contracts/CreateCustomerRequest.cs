namespace Customer.Api.Contracts
{
    public sealed record CreateCustomerRequest(
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
    );
}
