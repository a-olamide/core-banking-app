using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Dtos
{
    public sealed record HoldDto(
    Guid Id,
    decimal Amount,
    string Currency,
    string Reason,
    string Status,
    DateTimeOffset PlacedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset? ReleasedAt
);
}
