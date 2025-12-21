using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Abstractions.Persistence
{
    public interface IAccountReadOnlyRepository
    {
        Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken ct);
        Task<IReadOnlyList<AccountSummaryRow>> GetAccountsByCustomerIdAsync(Guid customerId, CancellationToken ct);
    }

    public sealed record AccountSummaryRow(
        Guid AccountId,
        string AccountNumber,
        string Currency,
        decimal BookBalance,
        decimal AvailableBalance,
        int Status
    );
}
