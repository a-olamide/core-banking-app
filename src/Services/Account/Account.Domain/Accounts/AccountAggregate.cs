using Account.Domain.ValueObjects;
using SharedKernel.Api;
using SharedKernel.Domain.Exceptions;
using SharedKernel.Domain.Primitives;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Accounts
{
    public sealed class AccountAggregate : AggregateRoot<Guid>
    {
        public Guid CustomerId { get; private set; }
        public AccountNumber AccountNumber { get; private set; } = default!;
        public AccountType AccountType { get; private set; }
        public string Currency { get; private set; } = "NGN";
        public AccountStatus Status { get; private set; }
        public Money BookBalance { get; private set; } = Money.From(0m, "NGN");
        public Money AvailableBalance { get; private set; } = Money.From(0m, "NGN");
        public DateTimeOffset CreatedAt { get; private set; }

        private readonly List<Hold> _holds = new();
        public IReadOnlyCollection<Hold> Holds => _holds.AsReadOnly();

        private readonly List<Lien> _liens = new();
        public IReadOnlyCollection<Lien> Liens => _liens.AsReadOnly();

        private AccountAggregate() { } // EF

        private AccountAggregate(Guid customerId, AccountNumber accountNumber, AccountType accountType, string currency)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            AccountNumber = accountNumber;
            AccountType = accountType;
            Currency = currency;
            Status = AccountStatus.Active;
            CreatedAt = DateTimeOffset.UtcNow;

            BookBalance = Money.From(0m, currency);
            AvailableBalance = Money.From(0m, currency);

            // Domain event later: AccountOpenedDomainEvent
            // AddDomainEvent(new AccountOpenedDomainEvent(...));
        }

        public static AccountAggregate Open(Guid customerId, AccountNumber accountNumber, AccountType accountType, string currency)
        {
            if (customerId == Guid.Empty)
                throw new DomainException(ErrorCodes.ValidationError, "CustomerId is required.");

            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException(ErrorCodes.ValidationError, "Currency is required.");

            currency = currency.Trim().ToUpperInvariant();

            return new AccountAggregate(customerId, accountNumber, accountType, currency);
        }

        public void Credit(Money amount)
        {
            EnsureActive();
            EnsureSameCurrency(amount);

            BookBalance = Money.From(BookBalance.Amount + amount.Amount, Currency);
            RecalculateAvailableBalance();
        }

        public void Debit(Money amount)
        {
            EnsureActive();
            EnsureSameCurrency(amount);

            // Debit is against AVAILABLE, not book
            if (AvailableBalance.Amount < amount.Amount)
                throw new DomainException("INSUFFICIENT_FUNDS", "Insufficient available balance.");

            BookBalance = Money.From(BookBalance.Amount - amount.Amount, Currency);
            RecalculateAvailableBalance();
        }

        public Guid PlaceHold(Money amount, string reason, DateTimeOffset? expiresAt = null)
        {
            EnsureActive();
            EnsureSameCurrency(amount);

            if (amount.Amount <= 0)
                throw new DomainException(ErrorCodes.ValidationError, "Hold amount must be greater than zero.");

            var now = DateTimeOffset.UtcNow;
            ExpireHoldsIfNeeded(now);

            var availableAfter = AvailableBalance.Amount - amount.Amount;
            if (availableAfter < 0)
                throw new DomainException("INSUFFICIENT_AVAILABLE_BALANCE", "Insufficient available balance to place hold.");

            var holdId = Guid.NewGuid();
            _holds.Add(Hold.Place(holdId, amount, reason, expiresAt));

            RecalculateAvailableBalance();

            // Domain event later: HoldPlacedDomainEvent
            return holdId;
        }

        public void ReleaseHold(Guid holdId, string? releaseReason = null)
        {
            var hold = _holds.SingleOrDefault(h => h.Id == holdId);
            if (hold is null)
                throw new DomainException(ErrorCodes.NotFound, "Hold not found.");

            hold.Release(releaseReason);
            RecalculateAvailableBalance();

            // Domain event later: HoldReleasedDomainEvent
        }

        public Guid PlaceLien(Money amount, string reason, string reference)
        {
            EnsureActive();
            EnsureSameCurrency(amount);

            if (amount.Amount <= 0)
                throw new DomainException(ErrorCodes.ValidationError, "Lien amount must be greater than zero.");

            var availableAfter = AvailableBalance.Amount - amount.Amount;
            if (availableAfter < 0)
                throw new DomainException("INSUFFICIENT_AVAILABLE_BALANCE", "Insufficient available balance to place lien.");

            var lienId = Guid.NewGuid();
            _liens.Add(Lien.Place(lienId, amount, reason, reference));

            RecalculateAvailableBalance();

            // Domain event later: LienPlacedDomainEvent
            return lienId;
        }

        public void ReleaseLien(Guid lienId, string? releaseReason = null)
        {
            var lien = _liens.SingleOrDefault(l => l.Id == lienId);
            if (lien is null)
                throw new DomainException(ErrorCodes.NotFound, "Lien not found.");

            lien.Release(releaseReason);
            RecalculateAvailableBalance();

            // Domain event later: LienReleasedDomainEvent
        }

        public void Freeze(string reason)
        {
            if (Status == AccountStatus.Closed)
                throw new DomainException("ACCOUNT_CLOSED", "Cannot freeze a closed account.");

            Status = AccountStatus.Frozen;
            // Domain event later
        }

        public void Unfreeze()
        {
            if (Status != AccountStatus.Frozen)
                return;

            Status = AccountStatus.Active;
        }

        public void Close()
        {
            Status = AccountStatus.Closed;
            // Domain event later
        }

        private void EnsureActive()
        {
            if (Status != AccountStatus.Active)
                throw new DomainException("ACCOUNT_NOT_ACTIVE", "Account is not active.");
        }

        private void EnsureSameCurrency(Money amount)
        {
            if (!string.Equals(amount.Currency, Currency, StringComparison.OrdinalIgnoreCase))
                throw new DomainException("CURRENCY_MISMATCH", "Currency mismatch.");
        }

        private void ExpireHoldsIfNeeded(DateTimeOffset now)
        {
            foreach (var h in _holds)
                h.MarkExpired(now);
        }

        private void RecalculateAvailableBalance()
        {
            var now = DateTimeOffset.UtcNow;
            ExpireHoldsIfNeeded(now);

            var activeHolds = _holds.Where(h => h.IsActive(now)).Sum(h => h.Amount.Amount);
            var activeLiens = _liens.Where(l => l.IsActive()).Sum(l => l.Amount.Amount);

            var available = BookBalance.Amount - activeHolds - activeLiens;

            if (available < 0)
                available = 0; // conservative; alternatively allow negative if overdraft is supported

            AvailableBalance = Money.From(available, Currency);
        }
    }
}
