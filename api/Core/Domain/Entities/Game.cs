using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Games")]
public class Game
{
    public int Id { get; set; }
    public string PreviewUrl { get; set; }
    public string Name { get; set; }
}