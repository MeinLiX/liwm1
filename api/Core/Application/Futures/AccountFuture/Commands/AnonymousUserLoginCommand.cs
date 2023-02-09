using Application.Interfaces;
using Domain.Responses;
using Domain.Responses.DTOs;
using FluentValidation;
using MediatR;

namespace Application.Futures.AccountFuture.Commands;

public class AnonymousUserLoginValidator : AbstractValidator<AnonymousUserLoginRequest>
{
    public AnonymousUserLoginValidator()
    {
        RuleFor(r => r.username).NotEmpty().WithMessage("Username must be filled")
                                .GreaterThanOrEqualTo("4").WithMessage("Username must be at least 4 symbols");
        RuleFor(r => r.photoId).NotEmpty().WithMessage("Photo id must be filled")
                               .GreaterThan(0).WithMessage("Photo id can be only greater or equal to 1");
    }
}

public class AnonymousUserLoginRequest : IRequest<IRestResponseResult<UserDetailWithTokenDTO>>
{
    public string username { get; set; }
    public int photoId { get; set; }
}

public class AnonymousUserLoginRequestHandler : IRequestHandler<AnonymousUserLoginRequest, IRestResponseResult<UserDetailWithTokenDTO>>
{
    private readonly IUserRepository userRepository;
    private readonly ITokenService tokenService;

    public AnonymousUserLoginRequestHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    public async Task<IRestResponseResult<UserDetailWithTokenDTO>> Handle(AnonymousUserLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await this.userRepository.GetAnonymousUserByUsernameAsync(request.username);

        if (user is not null)
        {
            return RestResponseResult<UserDetailWithTokenDTO>.Fail("User exists");
        }

        user = await this.userRepository.AddAnonymousUser(request.username, request.photoId);

        return RestResponseResult<UserDetailWithTokenDTO>.Success(new UserDetailWithTokenDTO(user)
        {
            Token = await this.tokenService.CreateTokenAsync(user)
        });
    }
}