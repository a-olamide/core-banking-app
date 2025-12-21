using Account.Application.Abstractions.Persistence;
using MediatR;
using SharedKernel.Api;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.ReleaseLien
{
    public sealed class ReleaseLienCommandHandler : IRequestHandler<ReleaseLienCommand>
    {
        private readonly IAccountRepository _accounts;
        private readonly IUnitOfWork _uow;

        public ReleaseLienCommandHandler(IAccountRepository accounts, IUnitOfWork uow)
        {
            _accounts = accounts;
            _uow = uow;
        }

        public async Task Handle(ReleaseLienCommand request, CancellationToken ct)
        {
            var account = await _accounts.GetByIdAsync(request.AccountId, ct)
                ?? throw new DomainException(ErrorCodes.NotFound, "Account not found.");

            account.ReleaseLien(request.LienId, request.ReleaseReason);

            await _uow.SaveChangesAsync(ct);
        }
    }
}
