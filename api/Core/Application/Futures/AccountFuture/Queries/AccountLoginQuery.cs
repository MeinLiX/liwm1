using Application.Interfaces;
using Domain.Responses;
using Domain.Responses.DTOs;
using FluentValidation;
using MediatR;

namespace Application.Futures.AccountFuture.Queries;

public class AccountLoginValidator : AbstractValidator<AccountLoginRequest>
{
    public AccountLoginValidator()
    {
        RuleFor(r => r.username).NotEmpty().WithMessage("Username must be filled");
        RuleFor(r => r.isAnonymous).NotNull().WithMessage("Is anonymous must filled");
    }
}

public class AccountLoginRequest : IRequest<IRestResponseResult<UserDetailWithTokenDTO>>
{
    public string username { get; set; }
    public string? password { get; set; }
    public int? photoId { get; set; }
    public bool isAnonymous { get; set; }
}

public class AccountLoginRequestHandler : IRequestHandler<AccountLoginRequest, IRestResponseResult<UserDetailWithTokenDTO>>
{
    private readonly IUserRepository userRepository;
    private readonly ITokenService tokenService;

    public AccountLoginRequestHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    public async Task<IRestResponseResult<UserDetailWithTokenDTO>> Handle(AccountLoginRequest request, CancellationToken cancellationToken)
    {
        dynamic? user;

        if (request.isAnonymous)
        {
            if (!request.photoId.HasValue)
            {
                return RestResponseResult<UserDetailWithTokenDTO>.Fail("Photo id must be provided for anonymous login");
            }

            user = await this.userRepository.GetAnonymousUserByUsernameAsync(request.username);

            if (user is not null)
            {
                return RestResponseResult<UserDetailWithTokenDTO>.Fail("User with provided username exists");
            }

            user = await this.userRepository.AddAnonymousUser(request.username, request.photoId.Value);
        }
        else
        {
            user = await this.userRepository.GetUserByUsernameAsync(request.username);

            if (user is null)
            {
                return RestResponseResult<UserDetailWithTokenDTO>.Fail("User with provided username not found");
            }

            if (!await this.userRepository.CheckPasswordForUserAsync(user, request.password))
            {
                return RestResponseResult<UserDetailWithTokenDTO>.Fail("Password is not correct");
            }
        }

        return RestResponseResult<UserDetailWithTokenDTO>.Success(new UserDetailWithTokenDTO(user)
        {
            Token = await this.tokenService.CreateTokenAsync(user)
        });
    }
}