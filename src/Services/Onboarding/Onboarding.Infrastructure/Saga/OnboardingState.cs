using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Infrastructure.Saga
{
    public sealed class OnboardingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        // MassTransit will store this as the current state name
        public string CurrentState { get; set; } = default!;

        public Guid? CustomerId { get; set; }
        public Guid? AccountId { get; set; }
        public string? AccountNumber { get; set; }
        public string? Currency { get; set; }
        public int? AccountType { get; set; }

        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
