using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class AppUser : IdentityUser<int>
{
    public int PhotoId { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public ICollection<AppUserRole> UserRoles { get; set; }
}