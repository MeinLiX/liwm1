using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IDataContext
{
    DbSet<AppUser> Users { get; set; }
    DbSet<AppRole> Roles { get; set; }
    DbSet<AppUserRole> UserRoles { get; set; }

    Task<int> SaveChangesAsync();
}