using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Lobbies")]
public class Lobby
{
    //TODO: Add limit to users and connections in lobby
    [Key]
    public int Id { get; set; }
    public string LobbyName { get; set; }
    public AppUser LobbyCreator { get; set; }
    public Game CurrentGame { get; set; }
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<Connection> Connections { get; set; } = new List<Connection>();
}