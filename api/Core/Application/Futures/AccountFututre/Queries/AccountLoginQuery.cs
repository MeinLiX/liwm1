using Domain.Responses;
using FluentValidation;
using MediatR;

namespace Application.Futures.AccountFuture.Queries;

public class AccountLoginValidator : AbstractValidator<AccountLoginRequest>
{
    public AccountLoginValidator()
    {
        RuleFor(r => r.username).NotEmpty().WithMessage("Username must be filled");
        RuleFor(r => r.password).NotEmpty().WithMessage("Password must be filled");
    }
}

public class AccountLoginRequest : IRequest<IRestResponseResult>
{
    public string username { get; set; }
    public string password { get; set; }
}

public class LoginRequestHandler : IRequestHandler<AccountLoginRequest, IRestResponseResult>
{
    public Task<IRestResponseResult> Handle(AccountLoginRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(RestResponseResult.Success("Pong!"));
    }
}

