using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;

namespace Domain.Entities;

[Table("Games")]
public class Game
{
    public int Id { get; set; }
    public GameMode GameMode { get; set; }
    public ICollection<AppUser> Players { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public GameState GameState { get; set; }
}