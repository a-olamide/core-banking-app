using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.OpenAccount
{
    public sealed class OpenAccountCommandValidator : AbstractValidator<OpenAccountCommand>
    {
        public OpenAccountCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Currency).NotEmpty().Length(3);
            RuleFor(x => x.AccountType).InclusiveBetween(1, 2); // Savings/Current
        }
    }
}
