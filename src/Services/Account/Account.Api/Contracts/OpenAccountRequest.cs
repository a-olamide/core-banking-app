using Account.Domain.Accounts;

namespace Account.Api.Contracts
{
    public sealed record OpenAccountRequest(
        Guid CustomerId,
        string Currency,
        AccountType AccountType
    );
}
