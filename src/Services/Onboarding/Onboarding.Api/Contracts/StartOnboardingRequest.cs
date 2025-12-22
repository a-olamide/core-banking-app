namespace Onboarding.Api.Contracts

{
    public sealed record StartOnboardingRequest(
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
    );
}
