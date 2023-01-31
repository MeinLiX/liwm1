using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.SqlLite.Contexts;

public class DataContext : IdentityDbContext<AppUser, AppRole, int,
                                            IdentityUserClaim<int>,
                                            AppUserRole,
                                            IdentityUserLogin<int>,
                                            IdentityRoleClaim<int>,
                                            IdentityUserToken<int>>,
                           IDataContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
               .HasMany(ur => ur.UserRoles)
               .WithOne(u => u.User)
               .HasForeignKey(ur => ur.UserId)
               .IsRequired();
    }

    public async Task<int> SaveChangesAsync() => await this.SaveChangesAsync();
}