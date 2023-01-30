using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task UpdateUserAsync(AppUser user);
    Task AddUserAsync(string username, string password, int photoId);
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
}