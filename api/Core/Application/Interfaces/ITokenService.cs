using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}