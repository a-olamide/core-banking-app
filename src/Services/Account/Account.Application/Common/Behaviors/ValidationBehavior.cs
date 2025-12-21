using FluentValidation;
using MediatR;
using SharedKernel.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = SharedKernel.Domain.Exceptions.ValidationException;

namespace Account.Application.Common.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
            => _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(cancellationToken);

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .GroupBy(
                    f => f.PropertyName,
                    f => f.ErrorMessage,
                    (property, errors) => new { property, errors = errors.Distinct().ToArray() })
                .ToDictionary(x => x.property, x => x.errors);

            if (failures.Count != 0)
                throw new AppValidationException(ErrorCodes.ValidationError, "Validation failed.", failures);

            return await next(cancellationToken);
        }
    }
}
