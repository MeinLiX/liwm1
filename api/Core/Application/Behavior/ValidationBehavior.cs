using System;
using FluentValidation;
using MediatR;

namespace Application.Behavior;


public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<IRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<IRequest>> validators) => _validators = validators;


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failrules = validationResults.SelectMany(v => v.Errors).Where(e => e != null).ToList();

            if (failrules.Any())
            {
                throw new ValidationException(failrules);
            }
        }
        return await next();
    }
}
