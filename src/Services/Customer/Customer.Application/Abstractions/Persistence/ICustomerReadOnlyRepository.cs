using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Abstractions.Persistence
{
    public interface ICustomerReadOnlyRepository
    {
        Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct);
    }
}
