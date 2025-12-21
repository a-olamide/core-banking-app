using Account.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Abstractions.Persistence
{
    public interface IAccountRepository
    {
        Task AddAsync(AccountAggregate account, CancellationToken ct);
        Task<AccountAggregate?> GetByIdAsync(Guid accountId, CancellationToken ct);
    }
}
