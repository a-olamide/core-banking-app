using Customer.Application.Abstractions.Persistence;
using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Infrastructure.Repositories
{
    public sealed class CustomerReadOnlyRepository : ICustomerReadOnlyRepository
    {
        private readonly CustomerDbContext _db;

        public CustomerReadOnlyRepository(CustomerDbContext db) => _db = db;

        public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct)
            => _db.Customers.AnyAsync(x => x.Email.Value == normalizedEmail, ct);
    }
}
