using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Accounts
{
    public sealed class Hold
    {
        public Guid Id { get; private set; }
        public Money Amount { get; private set; } = default!;
        public string Reason { get; private set; } = string.Empty;
        public HoldStatus Status { get; private set; }
        public DateTimeOffset PlacedAt { get; private set; }
        public DateTimeOffset? ExpiresAt { get; private set; }
        public DateTimeOffset? ReleasedAt { get; private set; }
        public string? ReleaseReason { get; private set; }

        private Hold() { } // EF

        private Hold(Guid id, Money amount, string reason, DateTimeOffset? expiresAt)
        {
            Id = id;
            Amount = amount;
            Reason = reason;
            ExpiresAt = expiresAt;
            PlacedAt = DateTimeOffset.UtcNow;
            Status = HoldStatus.Active;
        }

        public static Hold Place(Guid id, Money amount, string reason, DateTimeOffset? expiresAt)
            => new(id, amount, reason, expiresAt);

        public bool IsActive(DateTimeOffset now)
        {
            if (Status != HoldStatus.Active) return false;
            if (ExpiresAt is null) return true;
            return ExpiresAt.Value > now;
        }

        public void MarkExpired(DateTimeOffset now)
        {
            if (Status == HoldStatus.Active && ExpiresAt is not null && ExpiresAt.Value <= now)
            {
                Status = HoldStatus.Expired;
            }
        }

        public void Release(string? releaseReason = null)
        {
            if (Status != HoldStatus.Active)
                return;

            Status = HoldStatus.Released;
            ReleasedAt = DateTimeOffset.UtcNow;
            ReleaseReason = releaseReason;
        }
    }
}
