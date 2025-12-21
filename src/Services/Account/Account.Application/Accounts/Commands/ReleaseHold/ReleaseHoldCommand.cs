using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.ReleaseHold
{
    public sealed record ReleaseHoldCommand(
        Guid AccountId,
        Guid HoldId,
        string? ReleaseReason
    ) : IRequest;
}
