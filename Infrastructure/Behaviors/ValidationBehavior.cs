using FluentValidation;
using MediatR;
using OnSet.Infrastructure.Results;

namespace OnSet.Infrastructure.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();

                if (failures.Any())
                {
                    var errorMessages = failures.Select(f => f.ErrorMessage).Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();

                    // Commands: return Result.Invalid instead of throwing
                    if (typeof(TResponse) == typeof(Result))
                        return (TResponse)(object)Result.Invalid(errorMessages);

                    if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var invalidMethod = typeof(TResponse).GetMethod(nameof(Result<object>.Invalid), new[] { typeof(IEnumerable<string>) });
                        if (invalidMethod is not null)
                            return (TResponse)invalidMethod.Invoke(null, new object[] { errorMessages })!;
                    }

                    // Queries / other: keep exception-based flow
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}
