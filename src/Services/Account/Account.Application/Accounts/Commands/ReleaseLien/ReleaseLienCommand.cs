using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.ReleaseLien
{
    public sealed record ReleaseLienCommand(
        Guid AccountId,
        Guid LienId,
        string? ReleaseReason
    ) : IRequest;
}
