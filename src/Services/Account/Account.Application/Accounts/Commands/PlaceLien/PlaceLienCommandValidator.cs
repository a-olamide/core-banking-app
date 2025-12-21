using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Application.Accounts.Commands.PlaceLien
{
    public sealed class PlaceLienCommandValidator : AbstractValidator<PlaceLienCommand>
    {
        public PlaceLienCommandValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Currency).NotEmpty().Length(3);
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Reference).NotEmpty().MaximumLength(100);
        }
    }
}
