using Account.Application.Abstractions.Persistence;
using Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Infrastructure.Repositories
{
    public sealed class AccountReadOnlyRepository : IAccountReadOnlyRepository
    {
        private readonly AccountDbContext _db;

        public AccountReadOnlyRepository(AccountDbContext db) => _db = db;

        public Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken ct)
            => _db.Accounts.AnyAsync(a => a.AccountNumber.Value == accountNumber, ct);

        public async Task<IReadOnlyList<AccountSummaryRow>> GetAccountsByCustomerIdAsync(Guid customerId, CancellationToken ct)
        {
            return await _db.Accounts
                .Where(a => a.CustomerId == customerId)
                .Select(a => new AccountSummaryRow(
                    a.Id,
                    a.AccountNumber.Value,
                    a.Currency,
                    a.BookBalance.Amount,
                    a.AvailableBalance.Amount,
                    (int)a.Status
                ))
                .ToListAsync(ct);
        }
    }
}
