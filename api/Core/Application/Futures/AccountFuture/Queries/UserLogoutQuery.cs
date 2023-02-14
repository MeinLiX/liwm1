using Application.Interfaces;
using Domain.Entities;
using Domain.Responses;
using FluentValidation;
using MediatR;

namespace Application.Futures.AccountFuture.Commands;

public class UserLogoutValidator : AbstractValidator<UserLogoutRequest>
{
    public UserLogoutValidator()
    {
        RuleFor(r => r.username).NotEmpty().WithMessage("Username must be filled")
                                .GreaterThanOrEqualTo("4").WithMessage("Username must be at least 4 symbols");
    }
}

public class UserLogoutRequest : IRequest<IRestResponseResult>
{
    public string username { get; set; }
}

public class UserLogoutRequestHandler : IRequestHandler<UserLogoutRequest, IRestResponseResult>
{
    private readonly IUserRepository userRepository;
    private readonly ITokenService tokenService;

    public UserLogoutRequestHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    public async Task<IRestResponseResult> Handle(UserLogoutRequest request, CancellationToken cancellationToken)
    {
        var user = await this.userRepository.GetUserByUsernameAsync(request.username);

        if (user is null)
        {
            return RestResponseResult.Fail("User doesnt not exist");
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            await this.userRepository.LogoutFromAnonymousUserAsync(user);
        }

        return RestResponseResult.Success("User was logged out");
    }
}