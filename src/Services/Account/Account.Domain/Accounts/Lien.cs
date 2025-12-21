using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Accounts
{
    public sealed class Lien
    {
        public Guid Id { get; private set; }
        public Money Amount { get; private set; } = default!;
        public string Reason { get; private set; } = string.Empty;
        public string Reference { get; private set; } = string.Empty;
        public LienStatus Status { get; private set; }
        public DateTimeOffset PlacedAt { get; private set; }
        public DateTimeOffset? ReleasedAt { get; private set; }
        public string? ReleaseReason { get; private set; }

        private Lien() { } // EF

        private Lien(Guid id, Money amount, string reason, string reference)
        {
            Id = id;
            Amount = amount;
            Reason = reason;
            Reference = reference;
            PlacedAt = DateTimeOffset.UtcNow;
            Status = LienStatus.Active;
        }

        public static Lien Place(Guid id, Money amount, string reason, string reference)
            => new(id, amount, reason, reference);

        public bool IsActive() => Status == LienStatus.Active;

        public void Release(string? releaseReason = null)
        {
            if (Status != LienStatus.Active)
                return;

            Status = LienStatus.Released;
            ReleasedAt = DateTimeOffset.UtcNow;
            ReleaseReason = releaseReason;
        }
    }
}
