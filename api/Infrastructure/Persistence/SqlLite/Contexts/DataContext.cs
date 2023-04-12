using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SqlLite.Contexts;

public class DataContext : IdentityDbContext<AppUser, AppRole, int,
                                            IdentityUserClaim<int>,
                                            AppUserRole,
                                            IdentityUserLogin<int>,
                                            IdentityRoleClaim<int>,
                                            IdentityUserToken<int>>,
                           IDataContext
{
    public DbSet<Photo> Photos { get; set; }
    public DbSet<GameMode> GameModes { get; set; }
    public DbSet<Lobby> Lobbies { get; set; }
    public DbSet<RacingCar> RacingCars { get; set; }
    public DbSet<Game> Games { get; set; }

    public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
               .HasMany(user => user.UserRoles)
               .WithOne(ur => ur.User)
               .HasForeignKey(ur => ur.UserId)
               .IsRequired();

        builder.Entity<AppRole>()
               .HasMany(r => r.UserRoles)
               .WithOne(ur => ur.Role)
               .HasForeignKey(ur => ur.RoleId)
               .IsRequired();
    }

    public async Task<int> SaveChangesAsync() => await base.SaveChangesAsync();
}