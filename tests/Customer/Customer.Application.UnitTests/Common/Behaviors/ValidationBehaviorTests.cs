using Customer.Application.Common.Behaviors;
using Customer.Application.Customers.Command.CreateCustomer;
using FluentAssertions;
using FluentValidation;
using MediatR;
using SharedKernel.Web.Api;
using AppValidationException = SharedKernel.Domain.Exceptions.ValidationException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.UnitTests.Common.Behaviors
{
    public sealed class ValidationBehaviorTests
    {
        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            // Arrange
            var invalid = new CreateCustomerCommand(
                FirstName: "",
                MiddleName: null,
                LastName: "",
                Email: "bad-email",
                PhoneCountryCode: "",
                PhoneNumber: "",
                AddressLine1: "",
                AddressLine2: null,
                City: "",
                StateOrProvince: "",
                PostalCode: "",
                CountryCode: "N" // invalid length
            );

            var validator = new InlineValidator<CreateCustomerCommand>();
            validator.RuleFor(x => x.FirstName).NotEmpty();
            validator.RuleFor(x => x.Email).EmailAddress();

            var behavior = new ValidationBehavior<CreateCustomerCommand, string>(new[] { validator });

            // Fake "next"
            RequestHandlerDelegate<string> next = ct => Task.FromResult("OK");

            // Act
            var act = async () => await behavior.Handle(invalid, next, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<AppValidationException>();
            ex.Which.ErrorCode.Should().Be(ErrorCodes.ValidationError);
            ex.Which.Details.Should().ContainKey("FirstName");
            ex.Which.Details.Should().ContainKey("Email");
        }
    }
}
