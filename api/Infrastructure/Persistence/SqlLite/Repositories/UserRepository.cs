
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

    public async Task AddUserAsync(string username, string password, int photoId)
    {
        if (await this.GetUserByUsernameAsync(username) is null)
        {
            var user = new AppUser
            {
                UserName = username,
                PhotoId = photoId
            };

            await this.userManager.CreateAsync(user, password);
            await this.userManager.AddToRoleAsync(user, "Gamer");
        }
    }

    public async Task<AppUser?> GetUserByIdAsync(int id) => await this.dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<AppUser?> GetUserByUsernameAsync(string username) => await this.dataContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

    public Task UpdateUserAsync(AppUser user)
    {
        throw new NotImplementedException();
    }
}