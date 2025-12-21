using Account.Application.Abstractions.Persistence;
using Account.Application.Accounts.Dtos;
using MediatR;
using SharedKernel.Api;
using SharedKernel.Domain.Exceptions;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.PlaceHold
{
    public sealed class PlaceHoldCommandHandler : IRequestHandler<PlaceHoldCommand, HoldDto>
    {
        private readonly IAccountRepository _accounts;
        private readonly IUnitOfWork _uow;

        public PlaceHoldCommandHandler(IAccountRepository accounts, IUnitOfWork uow)
        {
            _accounts = accounts;
            _uow = uow;
        }

        public async Task<HoldDto> Handle(PlaceHoldCommand request, CancellationToken ct)
        {
            var account = await _accounts.GetByIdAsync(request.AccountId, ct)
                ?? throw new DomainException(ErrorCodes.NotFound, "Account not found.");

            var currency = request.Currency.Trim().ToUpperInvariant();
            var amount = Money.From(request.Amount, currency);

            var holdId = account.PlaceHold(amount, request.Reason, request.ExpiresAt);

            await _uow.SaveChangesAsync(ct);

            var hold = account.Holds.Single(h => h.Id == holdId);

            return new HoldDto(
                hold.Id,
                hold.Amount.Amount,
                hold.Amount.Currency,
                hold.Reason,
                hold.Status.ToString(),
                hold.PlacedAt,
                hold.ExpiresAt,
                hold.ReleasedAt
            );
        }
    }
}
