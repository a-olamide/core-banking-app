using SharedKernel.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Messaging.Commands
{
    public sealed record OpenAccountCommand(
        Guid CorrelationId,
        Guid CustomerId,
        string Currency,
        int AccountType
    ) : ICorrelatedMessage;
}
