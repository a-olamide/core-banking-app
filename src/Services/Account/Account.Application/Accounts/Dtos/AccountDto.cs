using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Dtos
{
    public sealed record AccountDto(
     Guid Id,
     Guid CustomerId,
     string AccountNumber,
     string Currency,
     decimal BookBalance,
     decimal AvailableBalance,
     string Status,
     DateTimeOffset CreatedAt
 );
}
