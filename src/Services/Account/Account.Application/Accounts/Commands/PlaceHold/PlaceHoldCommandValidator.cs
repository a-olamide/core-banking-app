using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.PlaceHold
{
    public sealed class PlaceHoldCommandValidator : AbstractValidator<PlaceHoldCommand>
    {
        public PlaceHoldCommandValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Currency).NotEmpty().Length(3);
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
        }
    }
}
