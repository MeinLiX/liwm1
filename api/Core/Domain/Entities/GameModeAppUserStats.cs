namespace Domain.Entities;

public class GameModeAppUserStats
{
    public int Id { get; set; }
    public int Level { get; private set; } //TODO: Came up with logic to calculate level
    public int Points { get; set; }
    public int PointsForNewLevel { get; set; }

    public int GameModeId { get; set; }
    public GameMode GameMode { get; set; }
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}