using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PostgreSQL.Contexts;

class DataContext : IdentityDbContext<AppUser, AppRole, int,
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
    public DbSet<GameAppUsersStats> GameAppUsersStats { get; set; }
    public DbSet<GameModeAppUserStats> GameModeAppUserStats { get; set; }

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

        builder.Entity<Lobby>()
               .HasMany(l => l.PreviousGames)
               .WithOne(g => g.Lobby)
               .HasForeignKey(g => g.LobbyId)
               .IsRequired();
    }

    public async Task<int> SaveChangesAsync()
        => await base.SaveChangesAsync();
}