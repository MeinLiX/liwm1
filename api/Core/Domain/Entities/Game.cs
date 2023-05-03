using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;

namespace Domain.Entities;

[Table("Games")]
public class Game
{
    public int Id { get; set; }
    public GameMode Mode { get; set; }
    public ICollection<AppUser> Players { get; set; }
    public ICollection<AppUser> RatingPlayers { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public GameState State { get; set; }
}