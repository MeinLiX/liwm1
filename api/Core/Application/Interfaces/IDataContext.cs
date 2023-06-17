using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces;

public interface IDataContext
{
    DatabaseFacade Database { get; }

    DbSet<AppUser> Users { get; set; }
    DbSet<AppRole> Roles { get; set; }
    DbSet<AppUserRole> UserRoles { get; set; }
    DbSet<Photo> Photos { get; set; }
    DbSet<GameMode> GameModes { get; set; }
    DbSet<Lobby> Lobbies { get; set; }
    DbSet<RacingCar> RacingCars { get; set; }
    DbSet<Game> Games { get; set; }
    DbSet<GameAppUsersStats> GameAppUsersStats { get; set; }

    Task<int> SaveChangesAsync();
}