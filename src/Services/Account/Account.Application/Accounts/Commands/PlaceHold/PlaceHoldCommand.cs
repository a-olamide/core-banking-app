using Account.Application.Accounts.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.PlaceHold
{
    public sealed record PlaceHoldCommand(
        Guid AccountId,
        decimal Amount,
        string Currency,
        string Reason,
        DateTimeOffset? ExpiresAt
    ) : IRequest<HoldDto>;
}
