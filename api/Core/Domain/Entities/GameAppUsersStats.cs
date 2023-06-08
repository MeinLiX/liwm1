namespace Domain.Entities;

public class GameAppUsersStats
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int AppUserId { get; set; }
    public int Place { get; set; }
    public Game Game { get; set; }
    public AppUser AppUser { get; set; }
}