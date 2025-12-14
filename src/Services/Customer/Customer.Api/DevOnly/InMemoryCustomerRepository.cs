using Customer.Application.Abstractions.Persistence;
using Customer.Domain.Customers;

namespace Customer.Api.DevOnly
{
    public sealed class InMemoryCustomerRepository :
    ICustomerRepository,
    ICustomerReadOnlyRepository,
    IUnitOfWork
    {
        private static readonly List<CustomerAggregate> Store = new();

        public Task AddAsync(CustomerAggregate customer, CancellationToken ct)
        {
            Store.Add(customer);
            return Task.CompletedTask;
        }

        public Task<CustomerAggregate?> GetByIdAsync(Guid customerId, CancellationToken ct)
            => Task.FromResult(Store.SingleOrDefault(x => x.Id == customerId));

        public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct)
            => Task.FromResult(Store.Any(x => x.Email.Value == normalizedEmail));

        public Task<int> SaveChangesAsync(CancellationToken ct)
            => Task.FromResult(1);
    }
}
