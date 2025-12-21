namespace Account.Api.Contracts
{
    public sealed record PlaceHoldRequest(
        decimal Amount,
        string Currency,
        string Reason,
        DateTimeOffset? ExpiresAt
    );
}
