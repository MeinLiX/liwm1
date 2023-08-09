using Domain.Entities;

namespace Application.Interfaces;

public interface ILevelsRepository
{
    public Task AddPointsAsync(AppUser user, GameMode gameMode, int addedPoints);
    public Task<IEnumerable<GameModeAppUserStats>> GetStatsForUserAsync(AppUser user);
    public Task<GameModeAppUserStats?> GetStatsForUserAsync(AppUser user, GameMode gameMode);
    public Task RemoveProgressAsync(AppUser user, GameMode gameMode);
    public Task RemoveProgressAsync(AppUser user, IEnumerable<GameMode> gameModes);
}