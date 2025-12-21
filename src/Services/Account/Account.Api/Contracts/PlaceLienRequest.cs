namespace Account.Api.Contracts
{
    public sealed record PlaceLienRequest(
        decimal Amount,
        string Currency,
        string Reason,
        string Reference
    );
}
