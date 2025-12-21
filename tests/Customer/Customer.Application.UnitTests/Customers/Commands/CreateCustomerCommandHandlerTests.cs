using Customer.Application.Abstractions.Persistence;
using Customer.Application.Customers.Command.CreateCustomer;
using Customer.Domain.Customers;
using FluentAssertions;
using Moq;
using SharedKernel.Api;
using SharedKernel.Domain.Exceptions;
using SharedKernel.Web.Api;

namespace Customer.Application.UnitTests.Customers.Commands
{
   
    public sealed class CreateCustomerCommandHandlerTests
        {
            private readonly Mock<ICustomerRepository> _repo = new();
            private readonly Mock<ICustomerReadOnlyRepository> _readRepo = new();
            private readonly Mock<IUnitOfWork> _uow = new();

            private CreateCustomerCommand CreateValidCommand() =>
                new(
                    FirstName: "Ola",
                    MiddleName: "B",
                    LastName: "Akin",
                    Email: "Ola.Akin@gmail.com",
                    PhoneCountryCode: "+234",
                    PhoneNumber: "8012345678",
                    AddressLine1: "12 Admiralty Way",
                    AddressLine2: null,
                    City: "Lagos",
                    StateOrProvince: "Lagos",
                    PostalCode: "100001",
                    CountryCode: "NG"
                );

            [Fact]
            public async Task Handle_ShouldCreateCustomer_WhenEmailDoesNotExist()
            {
                // Arrange
                var cmd = CreateValidCommand();

                _readRepo.Setup(x => x.EmailExistsAsync("ola.akin@gmail.com", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

                _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var handler = new CreateCustomerCommandHandler(_repo.Object, _readRepo.Object, _uow.Object);

                // Act
                var result = await handler.Handle(cmd, CancellationToken.None);

                // Assert
                result.Should().NotBeNull();
                result.Email.Should().Be("ola.akin@gmail.com"); // normalized
                result.FirstName.Should().Be("Ola");
                result.LastName.Should().Be("Akin");

                _repo.Verify(x => x.AddAsync(It.IsAny<CustomerAggregate>(), It.IsAny<CancellationToken>()), Times.Once);
                _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task Handle_ShouldThrowDomainException_WhenEmailAlreadyExists()
            {
                // Arrange
                var cmd = CreateValidCommand();

                _readRepo.Setup(x => x.EmailExistsAsync("ola.akin@gmail.com", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

                var handler = new CreateCustomerCommandHandler(_repo.Object, _readRepo.Object, _uow.Object);

                // Act
                var act = async () => await handler.Handle(cmd, CancellationToken.None);

                // Assert
                var ex = await act.Should().ThrowAsync<DomainException>();
                ex.Which.ErrorCode.Should().Be(ErrorCodes.CustomerEmailAlreadyExists);

                _repo.Verify(x => x.AddAsync(It.IsAny<CustomerAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
                _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            }
        }
    
}