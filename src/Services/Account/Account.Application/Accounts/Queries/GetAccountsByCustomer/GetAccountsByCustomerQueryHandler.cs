using Account.Application.Abstractions.Persistence;
using Account.Application.Accounts.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Queries.GetAccountsByCustomer
{
    public sealed class GetAccountsByCustomerQueryHandler : IRequestHandler<GetAccountsByCustomerQuery, IReadOnlyList<AccountDto>>
    {
        private readonly IAccountReadOnlyRepository _read;

        public GetAccountsByCustomerQueryHandler(IAccountReadOnlyRepository read) => _read = read;

        public async Task<IReadOnlyList<AccountDto>> Handle(GetAccountsByCustomerQuery request, CancellationToken ct)
        {
            var rows = await _read.GetAccountsByCustomerIdAsync(request.CustomerId, ct);

            return rows.Select(r => new AccountDto(
                r.AccountId,
                request.CustomerId,
                r.AccountNumber,
                r.Currency,
                r.BookBalance,
                r.AvailableBalance,
                ((Account.Domain.Accounts.AccountStatus)r.Status).ToString(),
                CreatedAt: default // you can include CreatedAt in row later
            )).ToList();
        }
    }
}
