using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("GameModes")]
public class GameMode
{
    [Key]
    public int Id { get; set; }
    public string PreviewUrl { get; set; }
    public string Name { get; set; }
    public ICollection<Lobby> Lobbies { get; set; }
    public ICollection<GameModeAppUserStats> Levels { get; set; }
}