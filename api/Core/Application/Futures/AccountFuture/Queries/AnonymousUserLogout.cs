using Application.Interfaces;
using Domain.Responses;
using FluentValidation;
using MediatR;

namespace Application.Futures.AccountFuture.Commands;

public class AnonymousUserLogoutValidator : AbstractValidator<AnonymousUserLogoutRequest>
{
    public AnonymousUserLogoutValidator()
    {
        RuleFor(r => r.username).NotEmpty().WithMessage("Username must be filled")
                                .GreaterThanOrEqualTo("4").WithMessage("Username must be at least 4 symbols");
    }
}

public class AnonymousUserLogoutRequest : IRequest<IRestResponseResult>
{
    public string username { get; set; }
}

public class AnonymousUserLogoutRequestHandler : IRequestHandler<AnonymousUserLogoutRequest, IRestResponseResult>
{
    private readonly IUserRepository userRepository;
    private readonly ITokenService tokenService;

    public AnonymousUserLogoutRequestHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    public async Task<IRestResponseResult> Handle(AnonymousUserLogoutRequest request, CancellationToken cancellationToken)
    {
        var user = await this.userRepository.GetAnonymousUserByUsernameAsync(request.username);

        if (user is null)
        {
            return RestResponseResult.Fail("User doest not exist");
        }

        await this.userRepository.LogoutFromAnonymousUserAsync(user);

        return RestResponseResult.Success("User was deleted");
    }
}