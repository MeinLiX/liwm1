using System.Text.RegularExpressions;
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
        RuleFor(r => r.username).NotEmpty().WithMessage("Username must be filled")
                                .Must(DoesNotContainCyrillicAndSpecial).WithMessage("Username must not contain cyrillic and special symbols");
        When(r => r.password is not null, () =>
        {
            RuleFor(r => r.password).Matches("(.*[a-z].*)").WithMessage("Password must have at least 1 lower case letter")
                                    .Matches("(.*[A-Z].*)").WithMessage("Password must have at least 1 upper case letter")
                                    .Matches(@"(.*\d.*)").WithMessage("Password must have at least 1 digit")
                                    .GreaterThanOrEqualTo("6").WithMessage("Password must be at least 6 symbols");
        });
    }

    private static bool DoesNotContainCyrillicAndSpecial(string input)
    {
        string pattern = @"[\p{IsCyrillic}]";
        bool containsCyrillic = System.Text.RegularExpressions.Regex.IsMatch(input, pattern);

        pattern = @"[\p{P}\p{S}]";
        bool containsSpecial = System.Text.RegularExpressions.Regex.IsMatch(input, pattern);

        return !(containsCyrillic || containsSpecial);
    }
}

public class AccountLoginRequest : IRequest<IRestResponseResult<UserDetailWithTokenDTO>>
{
    public string username { get; set; }
    public string? password { get; set; }
    public int? photoId { get; set; }
    public bool isAnonymous => password == null;
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
                if (user.Created.AddDays(1) >= DateTime.UtcNow)
                {
                    return RestResponseResult<UserDetailWithTokenDTO>.Fail("User with provided username exists");
                }
                else
                {
                    await this.userRepository.LogoutFromAnonymousUserAsync(user);
                }
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