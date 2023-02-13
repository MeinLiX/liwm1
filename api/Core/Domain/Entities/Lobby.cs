namespace Domain.Entities;

public class Lobby
{
    public int Id { get; set; }
    public Game Game { get; set; }
    public ICollection<AppUser> Users { get; set; }
}