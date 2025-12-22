namespace Onboarding.Api.Contracts
{
    public sealed record OnboardingStatusDto(
        Guid OperationId,
        string Status,
        Guid? CustomerId,
        Guid? AccountId,
        string? AccountNumber,
        string? ErrorCode,
        string? ErrorMessage
    );
}
