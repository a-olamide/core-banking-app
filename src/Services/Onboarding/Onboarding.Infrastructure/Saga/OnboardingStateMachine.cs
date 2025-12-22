using MassTransit;
using SharedKernel.Messaging.Commands;
using SharedKernel.Messaging.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Infrastructure.Saga
{
    public sealed class OnboardingStateMachine : MassTransitStateMachine<OnboardingState>
    {
        public State Submitted { get; private set; } = default!;
        public State CustomerCreated { get; private set; } = default!;
        public State Completed { get; private set; } = default!;
        public State Failed { get; private set; } = default!;

        public Event<StartOnboardingCommand> StartOnboarding { get; private set; } = default!;
        public Event<CustomerCreatedEvent> CustomerCreatedEvt { get; private set; } = default!;
        public Event<CustomerCreationFailedEvent> CustomerCreateFailedEvt { get; private set; } = default!;
        public Event<AccountOpenedEvent> AccountOpenedEvt { get; private set; } = default!;
        public Event<AccountOpenFailedEvent> AccountOpenFailedEvt { get; private set; } = default!;

        public OnboardingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            // correlate everything by CorrelationId (operationId)
            Event(() => StartOnboarding, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => CustomerCreatedEvt, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => CustomerCreateFailedEvt, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => AccountOpenedEvt, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => AccountOpenFailedEvt, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

            Initially(
                When(StartOnboarding)
                    .Then(ctx =>
                    {
                        ctx.Saga.CreatedAt = DateTimeOffset.UtcNow;
                        ctx.Saga.UpdatedAt = DateTimeOffset.UtcNow;
                        ctx.Saga.Currency = ctx.Message.Currency.Trim().ToUpperInvariant();
                        ctx.Saga.AccountType = ctx.Message.AccountType;
                    })
                    .TransitionTo(Submitted)
                    .Send(CreateCustomerCommandEndpoint(), ctx => new CreateCustomerCommand(
                        ctx.Message.CorrelationId,
                        ctx.Message.FirstName,
                        ctx.Message.MiddleName,
                        ctx.Message.LastName,
                        ctx.Message.Email,
                        ctx.Message.PhoneCountryCode,
                        ctx.Message.PhoneNumber,
                        ctx.Message.AddressLine1,
                        ctx.Message.AddressLine2,
                        ctx.Message.City,
                        ctx.Message.StateOrProvince,
                        ctx.Message.PostalCode,
                        ctx.Message.CountryCode
                    ))
            );

            During(Submitted,
                When(CustomerCreatedEvt)
                    .Then(ctx =>
                    {
                        ctx.Saga.CustomerId = ctx.Message.CustomerId;
                        ctx.Saga.UpdatedAt = DateTimeOffset.UtcNow;
                    })
                    .TransitionTo(CustomerCreated)
                    .Send(OpenAccountCommandEndpoint(), ctx => new OpenAccountCommand(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.CustomerId!.Value,
                        ctx.Saga.Currency!,
                        ctx.Saga.AccountType!.Value
                    ))
            );

            // Better: carry currency/accountType in saga (optional).
            // For demo, we’ll store them in saga by reading StartOnboarding message.
            // We'll fix that below with a simpler approach: use ctx.GetPayload in Initially (see note).
            // For now keep it simple; next revision will store currency/accountType.

            DuringAny(
                When(AccountOpenedEvt)
                    .Then(ctx =>
                    {
                        ctx.Saga.AccountId = ctx.Message.AccountId;
                        ctx.Saga.AccountNumber = ctx.Message.AccountNumber;
                        ctx.Saga.UpdatedAt = DateTimeOffset.UtcNow;
                    })
                    .TransitionTo(Completed)
                    .Finalize(),

                When(CustomerCreateFailedEvt)
                    .Then(ctx =>
                    {
                        ctx.Saga.ErrorCode = ctx.Message.ErrorCode;
                        ctx.Saga.ErrorMessage = ctx.Message.ErrorMessage;
                        ctx.Saga.UpdatedAt = DateTimeOffset.UtcNow;
                    })
                    .TransitionTo(Failed)
                    .Finalize(),

                When(AccountOpenFailedEvt)
                    .Then(ctx =>
                    {
                        ctx.Saga.ErrorCode = ctx.Message.ErrorCode;
                        ctx.Saga.ErrorMessage = ctx.Message.ErrorMessage;
                        ctx.Saga.UpdatedAt = DateTimeOffset.UtcNow;
                    })
                    .TransitionTo(Failed)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }

        // Route commands to the proper queues (MassTransit endpoint convention)
        // IMPORTANT: these queue names must match consumer endpoint names in Customer/Account services.
        private static Uri CreateCustomerCommandEndpoint()
            => new("queue:create-customer-command-consumer");

        private static Uri OpenAccountCommandEndpoint()
            => new("queue:open-account-command-consumer");
    }
}
