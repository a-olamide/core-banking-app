using Account.Application.Accounts.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.PlaceLien
{
    public sealed record PlaceLienCommand(
        Guid AccountId,
        decimal Amount,
        string Currency,
        string Reason,
        string Reference
    ) : IRequest<LienDto>;
}
