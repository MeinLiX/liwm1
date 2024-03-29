using System.Text.RegularExpressions;
using Application.Interfaces;
using Domain.Responses;
using Domain.Responses.DTOs;
using FluentValidation;
using MediatR;

namespace Application.Futures.AccountFuture.Commands;

public class AccountRegisterValidator : AbstractValidator<AccountRegisterRequest>
{
    public AccountRegisterValidator()
    {
        RuleFor(r => r.username).NotEmpty().WithMessage("Username must be filled")
                                .Must(DoesNotContainCyrillicAndSpecial).WithMessage("Username must not contain cyrillic and special symbols")
                                .GreaterThanOrEqualTo("4").WithMessage("Username must be at least 4 symbols");
        RuleFor(r => r.password).NotEmpty().WithMessage("Password must be filled")
                                .Matches("(.*[a-z].*)").WithMessage("Password must have at least 1 lower case letter")
                                .Matches("(.*[A-Z].*)").WithMessage("Password must have at least 1 upper case letter")
                                .Matches(@"(.*\d.*)").WithMessage("Password must have at least 1 digit")
                                .GreaterThanOrEqualTo("6").WithMessage("Password must be at least 6 symbols");
        RuleFor(r => r.photoId).NotEmpty().WithMessage("Photo id must be filled")
                               .GreaterThan(0).WithMessage("Photo id can be only greater or equal to 1");
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

public class AccountRegisterRequest : IRequest<IRestResponseResult<UserDetailWithTokenDTO>>
{
    public string username { get; set; }
    public string password { get; set; }
    public int photoId { get; set; }
}

public class AccountRegisterRequestHandler : IRequestHandler<AccountRegisterRequest, IRestResponseResult<UserDetailWithTokenDTO>>
{
    private readonly IUserRepository userRepository;
    private readonly ITokenService tokenService;

    public AccountRegisterRequestHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    public async Task<IRestResponseResult<UserDetailWithTokenDTO>> Handle(AccountRegisterRequest request, CancellationToken cancellationToken)
    {
        var user = await this.userRepository.GetUserByUsernameAsync(request.username);

        if (user is not null)
        {
            return RestResponseResult<UserDetailWithTokenDTO>.Fail("User exists");
        }

        user = await this.userRepository.AddUserAsync(request.username, request.password, request.photoId);

        return RestResponseResult<UserDetailWithTokenDTO>.Success(new UserDetailWithTokenDTO(user)
        {
            Token = await this.tokenService.CreateTokenAsync(user)
        });
    }
}