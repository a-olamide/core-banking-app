using Customer.Application.Abstractions.Persistence;
using Customer.Domain.Customers;
using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Infrastructure.Repositories
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _db;

        public CustomerRepository(CustomerDbContext db) => _db = db;

        public async Task AddAsync(CustomerAggregate customer, CancellationToken ct)
            => await _db.Customers.AddAsync(customer, ct);

        public Task<CustomerAggregate?> GetByIdAsync(Guid customerId, CancellationToken ct)
            => _db.Customers.FirstOrDefaultAsync(x => x.Id == customerId, ct);
    }
}
