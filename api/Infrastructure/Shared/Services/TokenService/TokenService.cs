using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Services.TokenService;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey key;
    private readonly UserManager<AppUser> userManager;

    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        this.userManager = userManager;
    }

    public async Task<string> CreateTokenAsync(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };

        var roles = await this.userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(this.key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}