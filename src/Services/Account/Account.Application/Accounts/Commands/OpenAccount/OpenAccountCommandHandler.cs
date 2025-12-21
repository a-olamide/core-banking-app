using Account.Application.Abstractions.Persistence;
using Account.Application.Abstractions.Services;
using Account.Application.Accounts.Dtos;
using Account.Domain.Accounts;
using Account.Domain.ValueObjects;
using MediatR;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.OpenAccount
{
    public sealed class OpenAccountCommandHandler : IRequestHandler<OpenAccountCommand, AccountDto>
    {
        private readonly IAccountRepository _accounts;
        private readonly IAccountReadOnlyRepository _accountsRead;
        private readonly IAccountNumberGenerator _accountNumberGenerator;
        private readonly IUnitOfWork _uow;

        public OpenAccountCommandHandler(
            IAccountRepository accounts,
            IAccountReadOnlyRepository accountsRead,
            IAccountNumberGenerator accountNumberGenerator,
            IUnitOfWork uow)
        {
            _accounts = accounts;
            _accountsRead = accountsRead;
            _accountNumberGenerator = accountNumberGenerator;
            _uow = uow;
        }

        public async Task<AccountDto> Handle(OpenAccountCommand request, CancellationToken ct)
        {
            var currency = request.Currency.Trim().ToUpperInvariant();

            // Generate account number with uniqueness check (defense-in-depth)
            string? number = null;
            for (var attempt = 0; attempt < 5; attempt++)
            {
                var candidate = await _accountNumberGenerator.Generate10DigitAsync(ct);
                if (!await _accountsRead.AccountNumberExistsAsync(candidate, ct))
                {
                    number = candidate;
                    break;
                }
            }

            if (number is null)
                throw new DomainException("ACCOUNT_NUMBER_GENERATION_FAILED", "Failed to generate a unique account number.");

            var accountNumber = AccountNumber.Create(number);
            var accountType = (AccountType)request.AccountType;

            var account = AccountAggregate.Open(request.CustomerId, accountNumber, accountType, currency);

            await _accounts.AddAsync(account, ct);
            await _uow.SaveChangesAsync(ct);

            return new AccountDto(
                account.Id,
                account.CustomerId,
                account.AccountNumber.Value,
                account.Currency,
                account.BookBalance.Amount,
                account.AvailableBalance.Amount,
                account.Status.ToString(),
                account.CreatedAt
            );
        }
    }
}
