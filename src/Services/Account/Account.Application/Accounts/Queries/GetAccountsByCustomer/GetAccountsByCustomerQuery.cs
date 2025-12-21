using Account.Application.Accounts.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Queries.GetAccountsByCustomer
{
    public sealed record GetAccountsByCustomerQuery(Guid CustomerId) : IRequest<IReadOnlyList<AccountDto>>;

}
