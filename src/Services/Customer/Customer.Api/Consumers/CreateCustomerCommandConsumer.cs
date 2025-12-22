using MassTransit;
using MediatR;
using SharedKernel.Domain.Exceptions;
using MessagingCreateCustomerCommand = SharedKernel.Messaging.Commands.CreateCustomerCommand;
using AppCreateCustomerCommand = Customer.Application.Customers.Command.CreateCustomer.CreateCustomerCommand;

using SharedKernel.Messaging.Events;

namespace Customer.Api.Consumers
{
    public sealed class CreateCustomerCommandConsumer : IConsumer<MessagingCreateCustomerCommand>
    {
        private readonly IMediator _mediator;

        public CreateCustomerCommandConsumer(IMediator mediator) => _mediator = mediator;

        public async Task Consume(ConsumeContext<MessagingCreateCustomerCommand> context)
        {
            try
            {
                var m = context.Message;

                var result = await _mediator.Send(new AppCreateCustomerCommand(
                    m.FirstName,
                    m.MiddleName,
                    m.LastName,
                    m.Email,
                    m.PhoneCountryCode,
                    m.PhoneNumber,
                    m.AddressLine1,
                    m.AddressLine2,
                    m.City,
                    m.StateOrProvince,
                    m.PostalCode,
                    m.CountryCode
                ), context.CancellationToken);

                await context.Publish(new CustomerCreatedEvent(
                    m.CorrelationId,
                    result.Id
                ));
            }
            catch (DomainException ex)
            {
                await context.Publish(new CustomerCreationFailedEvent(
                    context.Message.CorrelationId,
                    ex.ErrorCode,
                    ex.Message
                ));
            }
            catch (Exception)
            {
                await context.Publish(new CustomerCreationFailedEvent(
                    context.Message.CorrelationId,
                    "CUSTOMER_CREATE_FAILED",
                    "Customer creation failed."
                ));
            }
        }
    }
}
