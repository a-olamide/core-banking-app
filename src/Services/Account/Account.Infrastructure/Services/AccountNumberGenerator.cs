using Account.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Account.Infrastructure.Services
{

    public sealed class AccountNumberGenerator : IAccountNumberGenerator
    {
        public Task<string> Generate10DigitAsync(CancellationToken ct)
        {
            // 10 digits, leading zeros allowed
            // Uses cryptographically strong RNG
            var bytes = new byte[8];
            RandomNumberGenerator.Fill(bytes);

            var value = BitConverter.ToUInt64(bytes, 0) % 10_000_000_000UL;
            var accountNumber = value.ToString("D10");

            return Task.FromResult(accountNumber);
        }
    }
}
