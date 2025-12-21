using Customer.Application.Abstractions.Persistence;
using Customer.Application.Customers.Dtos;
using Customer.Domain.Customers;
using MediatR;
using SharedKernel.Api;
using SharedKernel.Domain.Exceptions;
using SharedKernel.ValueObjects;
using SharedKernel.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Customers.Command.CreateCustomer
{
    public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
    {
        private readonly ICustomerRepository _customers;
        private readonly ICustomerReadOnlyRepository _customersRead;
        private readonly IUnitOfWork _uow;

        public CreateCustomerCommandHandler(
            ICustomerRepository customers,
            ICustomerReadOnlyRepository customersRead,
            IUnitOfWork uow)
        {
            _customers = customers;
            _customersRead = customersRead;
            _uow = uow;
        }

        public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken ct)
        {
            // Normalize email exactly as Email VO will normalize it
            var email = Email.Create(request.Email);

            // Cross-aggregate uniqueness rule (belongs in application/infrastructure boundary)
            var emailExists = await _customersRead.EmailExistsAsync(email.Value, ct);
            if (emailExists)
                throw new DomainException(ErrorCodes.CustomerEmailAlreadyExists, "Email already exists.");

            var name = PersonName.Create(request.FirstName, request.MiddleName, request.LastName);
            var phone = PhoneNumber.Create(request.PhoneCountryCode, request.PhoneNumber);
            var address = Address.Create(
                request.AddressLine1,
                request.AddressLine2,
                request.City,
                request.StateOrProvince,
                request.PostalCode,
                request.CountryCode
            );

            var customer = CustomerAggregate.Create(name, email, phone, address);

            await _customers.AddAsync(customer, ct);
            await _uow.SaveChangesAsync(ct);

            return new CustomerDto(
                customer.Id,
                customer.Name.FirstName,
                customer.Name.MiddleName,
                customer.Name.LastName,
                customer.Email.Value,
                customer.PhoneNumber.CountryCode,
                customer.PhoneNumber.Number,
                customer.Address.CountryCode,
                customer.Address.City
            );
        }
    }
}
