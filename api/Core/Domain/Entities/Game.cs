using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;

namespace Domain.Entities;

[Table("Games")]
public class Game
{
    public int Id { get; set; }
    public GameMode Mode { get; set; }
    public ICollection<AppUser> Players { get; set; } = new List<AppUser>();
    public ICollection<GameAppUsersStats> Stats { get; set; } = new List<GameAppUsersStats>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public GameState State { get; set; }
    public int LobbyId { get; set; }
    public Lobby Lobby { get; set; }
}