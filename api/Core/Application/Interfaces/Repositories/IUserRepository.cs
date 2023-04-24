using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task LogoutFromAnonymousUserAsync(AppUser user);
    Task<AppUser> AddUserAsync(string username, int photoId);
    Task<AppUser> AddUserAsync(string username, string password, int photoId);
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<string>> GetRolesForUserAsync(AppUser user);
    Task<bool> CheckPasswordForUserAsync(AppUser user, string password);
}