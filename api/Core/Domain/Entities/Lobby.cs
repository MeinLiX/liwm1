using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Lobbies")]
public class Lobby
{
    public int Id { get; set; }
    public Game Game { get; set; }
    public ICollection<AppUser> Users { get; set; }
}