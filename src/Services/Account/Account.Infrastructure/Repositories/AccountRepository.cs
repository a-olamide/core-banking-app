using Account.Application.Abstractions.Persistence;
using Account.Domain.Accounts;
using Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Infrastructure.Repositories
{
    public sealed class AccountRepository : IAccountRepository
    {
        private readonly AccountDbContext _db;

        public AccountRepository(AccountDbContext db) => _db = db;

        public async Task AddAsync(AccountAggregate account, CancellationToken ct)
            => await _db.Accounts.AddAsync(account, ct);

        public Task<AccountAggregate?> GetByIdAsync(Guid accountId, CancellationToken ct)
            => _db.Accounts
                .Include("_holds")
                .Include("_liens")
                .FirstOrDefaultAsync(a => a.Id == accountId, ct);
    }
}
