using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey key;
    private readonly IUserRepository userRepository;

    public TokenService(IConfiguration config, IUserRepository userRepository)
    {
        key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        this.userRepository = userRepository;
    }

    public async Task<string> CreateTokenAsync(AppUser user) => await this.CreateTokenAsync(user.Id, user.UserName, await userRepository.GetRolesForUserAsync(user), DateTime.Now.AddDays(7));

    public async Task<string> CreateTokenAsync(AnonymousUser user) => await this.CreateTokenAsync(user.Id, user.UserName, new string[] { user.Role }, DateTime.Now.AddDays(1));

    private Task<string> CreateTokenAsync(int id, string username, IEnumerable<string> roles, DateTime expireDate)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(this.key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expireDate,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Task.FromResult(tokenHandler.WriteToken(token));
    }
}