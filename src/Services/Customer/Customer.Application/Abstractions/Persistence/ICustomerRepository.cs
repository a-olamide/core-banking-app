using Customer.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Abstractions.Persistence
{
    public interface ICustomerRepository
    {
        Task AddAsync(CustomerAggregate customer, CancellationToken ct);
        Task<CustomerAggregate?> GetByIdAsync(Guid customerId, CancellationToken ct);
    }
}
