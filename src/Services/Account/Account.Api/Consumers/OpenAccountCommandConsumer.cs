using MassTransit;
using MediatR;
using SharedKernel.Domain.Exceptions;
using MessagingOpenAccountCommand = SharedKernel.Messaging.Commands.OpenAccountCommand;
using SharedKernel.Messaging.Events;
using Account.Application.Accounts.Commands.OpenAccount;

namespace Account.Api.Consumers
{
    public sealed class OpenAccountCommandConsumer : IConsumer<MessagingOpenAccountCommand>
    {
        private readonly IMediator _mediator;

        public OpenAccountCommandConsumer(IMediator mediator) => _mediator = mediator;

        public async Task Consume(ConsumeContext<MessagingOpenAccountCommand> context)
        {
            try
            {
                var m = context.Message;

                var dto = await _mediator.Send(
                    new OpenAccountCommand(m.CustomerId, m.Currency, m.AccountType),
                    context.CancellationToken);

                await context.Publish(new AccountOpenedEvent(
                    m.CorrelationId,
                    dto.Id,
                    dto.AccountNumber
                ));
            }
            catch (DomainException ex)
            {
                await context.Publish(new AccountOpenFailedEvent(
                    context.Message.CorrelationId,
                    ex.ErrorCode,
                    ex.Message
                ));
            }
            catch (Exception)
            {
                await context.Publish(new AccountOpenFailedEvent(
                    context.Message.CorrelationId,
                    "ACCOUNT_OPEN_FAILED",
                    "Account opening failed."
                ));
            }
        }
    }
}
