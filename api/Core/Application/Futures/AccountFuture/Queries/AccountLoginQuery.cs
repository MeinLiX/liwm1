using Application.Interfaces;
using Domain.Entities;
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
        When(r => r.password is not null, () =>
        {
            RuleFor(r => r.password).Matches("(.*[a-z].*)").WithMessage("Password must have at least 1 lower case letter")
                                    .Matches("(.*[A-Z].*)").WithMessage("Password must have at least 1 upper case letter")
                                    .Matches(@"(.*\d.*)").WithMessage("Password must have at least 1 digit")
                                    .GreaterThanOrEqualTo("6").WithMessage("Password must be at least 6 symbols");
        });
    }
}

public class AccountLoginRequest : IRequest<IRestResponseResult<UserDetailWithTokenDTO>>
{
    public string username { get; set; }
    public string? password { get; set; }
    public int? photoId { get; set; }
    public bool isAnonymous => string.IsNullOrEmpty(password);
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
        AppUser? user;

        if (request.isAnonymous)
        {
            if (!request.photoId.HasValue)
            {
                return RestResponseResult<UserDetailWithTokenDTO>.Fail("Photo id must be provided for anonymous login");
            }

            user = await this.userRepository.GetUserByUsernameAsync(request.username);

            if (user is not null)
            {
                return RestResponseResult<UserDetailWithTokenDTO>.Fail("User with provided username exists");
            }

            user = await this.userRepository.AddUserAsync(request.username, request.photoId.Value);
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