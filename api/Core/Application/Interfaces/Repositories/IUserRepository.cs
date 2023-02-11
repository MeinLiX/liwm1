using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<AnonymousUser> AddAnonymousUser(string username, int photoId);
    Task<AnonymousUser?> GetAnonymousUserByIdAsync(int id);
    Task<AnonymousUser?> GetAnonymousUserByUsernameAsync(string username);
    Task LogoutFromAnonymousUserAsync(AnonymousUser user);
    Task UpdateUserAsync(AppUser user);
    Task<AppUser> AddUserAsync(string username, string password, int photoId);
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<string>> GetRolesForUserAsync(AppUser user);
    Task<bool> CheckPasswordForUserAsync(AppUser user, string password);
}