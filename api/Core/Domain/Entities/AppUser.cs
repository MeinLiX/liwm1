using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class AppUser : IdentityUser<int>
{
    //TODO: Rating game ID Game id
    public int PhotoId { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public ICollection<AppUserRole> UserRoles { get; set; }
    public ICollection<GameAppUsersStats> Stats { get; set; }
}