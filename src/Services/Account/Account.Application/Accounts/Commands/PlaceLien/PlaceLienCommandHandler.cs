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

namespace Account.Application.Accounts.Commands.PlaceLien
{
    public sealed class PlaceLienCommandHandler : IRequestHandler<PlaceLienCommand, LienDto>
    {
        private readonly IAccountRepository _accounts;
        private readonly IUnitOfWork _uow;

        public PlaceLienCommandHandler(IAccountRepository accounts, IUnitOfWork uow)
        {
            _accounts = accounts;
            _uow = uow;
        }

        public async Task<LienDto> Handle(PlaceLienCommand request, CancellationToken ct)
        {
            var account = await _accounts.GetByIdAsync(request.AccountId, ct)
                ?? throw new DomainException(ErrorCodes.NotFound, "Account not found.");

            var currency = request.Currency.Trim().ToUpperInvariant();
            var amount = Money.From(request.Amount, currency);

            var lienId = account.PlaceLien(amount, request.Reason, request.Reference);

            await _uow.SaveChangesAsync(ct);

            var lien = account.Liens.Single(l => l.Id == lienId);

            return new LienDto(
                lien.Id,
                lien.Amount.Amount,
                lien.Amount.Currency,
                lien.Reason,
                lien.Reference,
                lien.Status.ToString(),
                lien.PlacedAt,
                lien.ReleasedAt
            );
        }
    }
}
