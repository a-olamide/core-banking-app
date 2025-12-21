using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Abstractions.Services
{
    public interface IAccountNumberGenerator
    {
        Task<string> Generate10DigitAsync(CancellationToken ct);
    }
}
