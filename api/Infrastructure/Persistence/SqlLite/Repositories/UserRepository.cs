using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SqlLite.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDataContext dataContext;
    private readonly UserManager<AppUser> userManager;

    public UserRepository(IDataContext dataContext, UserManager<AppUser> userManager)
    {
        this.dataContext = dataContext;
        this.userManager = userManager;
    }

    public async Task<AnonymousUser> AddAnonymousUser(string username, int photoId)
    {
        var user = new AnonymousUser 
        {
            UserName = username,
            PhotoId = photoId
        };

        await this.dataContext.AnonymousUsers.AddAsync(user);   
        await this.dataContext.SaveChangesAsync();

        return user;
    }

    public async Task<AppUser> AddUserAsync(string username, string password, int photoId)
    {
        var user = new AppUser
        {
            UserName = username,
            PhotoId = photoId
        };

        await this.userManager.CreateAsync(user, password);
        await this.userManager.AddToRoleAsync(user, "Gamer");

        return user;
    }

    public async Task<bool> CheckPasswordForUserAsync(AppUser user, string password) => await this.userManager.CheckPasswordAsync(user, password);

    public async Task<AnonymousUser?> GetAnonymousUserByIdAsync(int id) => await this.dataContext.AnonymousUsers.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<AnonymousUser?> GetAnonymousUserByUsernameAsync(string username) => await this.dataContext.AnonymousUsers.FirstOrDefaultAsync(u => u.UserName == username);

    public async Task<IEnumerable<string>> GetRolesForUserAsync(AppUser user) => await this.userManager.GetRolesAsync(user);

    public async Task<AppUser?> GetUserByIdAsync(int id) => await this.dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<AppUser?> GetUserByUsernameAsync(string username) => await this.dataContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

    public async Task LogoutFromAnonymousUserAsync(AnonymousUser user)
    {
        this.dataContext.AnonymousUsers.Remove(user);
        await this.dataContext.SaveChangesAsync();
    }

    public Task UpdateUserAsync(AppUser user)
    {
        throw new NotImplementedException();
    }
}