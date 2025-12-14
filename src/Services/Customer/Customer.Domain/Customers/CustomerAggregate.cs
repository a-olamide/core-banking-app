using Customer.Domain.Customers.DomainEvents;
using SharedKernel.Domain.Primitives;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Domain.Customers
{
    public sealed class CustomerAggregate : AggregateRoot<Guid>
    {
        public PersonName Name { get; private set; } = default!;
        public Email Email { get; private set; } = default!;
        public PhoneNumber PhoneNumber { get; private set; } = default!;
        public Address Address { get; private set; } = default!;
        public CustomerStatus Status { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private CustomerAggregate() { } // For EF Core

        private CustomerAggregate(PersonName name, Email email, PhoneNumber phoneNumber, Address address)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;

            Status = CustomerStatus.Active;
            CreatedAt = DateTimeOffset.UtcNow;

            AddDomainEvent(new CustomerCreatedDomainEvent(Id, Email.Value));
        }

        public static CustomerAggregate Create(PersonName name, Email email, PhoneNumber phoneNumber, Address address)
            => new(name, email, phoneNumber, address);

        public void UpdateContact(PhoneNumber phoneNumber, Address address)
        {
            PhoneNumber = phoneNumber;
            Address = address;
            // AddDomainEvent(new CustomerContactUpdatedDomainEvent(Id));
        }

        public void Suspend(string reason)
        {
            if (Status == CustomerStatus.Closed)
                throw new InvalidOperationException("Cannot suspend a closed customer.");

            Status = CustomerStatus.Suspended;
            // AddDomainEvent(...)
        }

        public void Close()
        {
            Status = CustomerStatus.Closed;
            // AddDomainEvent(...)
        }
    }
}
