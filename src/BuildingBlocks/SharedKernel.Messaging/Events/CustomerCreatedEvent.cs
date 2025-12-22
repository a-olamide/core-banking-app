using SharedKernel.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Messaging.Events
{
    public sealed record CustomerCreatedEvent(
    Guid CorrelationId,
    Guid CustomerId) : ICorrelatedMessage;
}
