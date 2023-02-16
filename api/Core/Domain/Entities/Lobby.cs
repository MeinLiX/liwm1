using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Lobbies")]
public class Lobby
{
    [Key]
    public int Id { get; set; }
    public ICollection<AppUser> Users { get; set; }
}