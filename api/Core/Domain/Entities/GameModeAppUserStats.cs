namespace Domain.Entities;

public class GameModeAppUserStats
{
    public int Id { get; set; }
    public int Level
    {
        get => Convert.ToInt32(Math.Floor((double)this.Points / (double)this.TotalPointsForNewLevel));
    }
    public int Points { get; set; }
    public int TotalPointsForNewLevel { get; set; }

    public GameMode GameMode { get; set; }
    public AppUser AppUser { get; set; }
}