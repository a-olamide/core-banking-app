using Account.Application.Accounts.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.OpenAccount
{
    public sealed record OpenAccountCommand(
    Guid CustomerId,
    string Currency,
    int AccountType // map to enum in handler
) : IRequest<AccountDto>;
}
