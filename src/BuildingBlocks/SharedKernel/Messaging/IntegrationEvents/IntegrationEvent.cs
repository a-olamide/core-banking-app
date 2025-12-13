using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Messaging.IntegrationEvents
{
    public abstract record IntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;

        // Optional but strongly recommended for tracing/correlation across services
        public string? CorrelationId { get; init; }
        public string? CausationId { get; init; }
    }
}
