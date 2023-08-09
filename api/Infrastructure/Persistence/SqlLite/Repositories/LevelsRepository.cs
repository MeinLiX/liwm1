using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SqlLite.Repositories;

public class LevelsRepository : ILevelsRepository
{
    private readonly IDataContext dataContext;

    public LevelsRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task AddPointsAsync(AppUser user, GameMode gameMode, int addedPoints)
    {
        var stats = await this.dataContext.GameModeAppUserStats.FirstOrDefaultAsync(s => s.GameMode == gameMode && s.AppUser == user);
        if (stats is null)
        {
            stats = new GameModeAppUserStats
            {
                AppUser = user,
                AppUserId = user.Id,
                GameMode = gameMode,
                GameModeId = gameMode.Id,
                Points = 0,
                TotalPointsForNewLevel = 1000
            };
            await this.dataContext.GameModeAppUserStats.AddAsync(stats);
        }

        stats.Points += addedPoints;
        if (stats.Points > stats.TotalPointsForNewLevel)
        {
            stats.TotalPointsForNewLevel += 1000;
        }
        await this.dataContext.SaveChangesAsync();
    }

    public Task<IEnumerable<GameModeAppUserStats>> GetStatsForUserAsync(AppUser user)
        => Task.FromResult(this.dataContext.GameModeAppUserStats.Where(s => s.AppUser == user).AsEnumerable());

    public async Task<GameModeAppUserStats?> GetStatsForUserAsync(AppUser user, GameMode gameMode)
        => await this.dataContext.GameModeAppUserStats.FirstOrDefaultAsync(s => s.AppUser == user && s.GameMode == gameMode);

    public async Task RemoveProgressAsync(AppUser user, GameMode gameMode)
    {
        var stats = await this.dataContext.GameModeAppUserStats.FirstOrDefaultAsync(s => s.GameMode == gameMode && s.AppUser == user);
        if (stats is not null)
        {
            this.dataContext.GameModeAppUserStats.Remove(stats);
            await this.dataContext.SaveChangesAsync();
        }
    }

    public async Task RemoveProgressAsync(AppUser user, IEnumerable<GameMode> gameModes)
    {
        var stats = this.dataContext.GameModeAppUserStats.Where(s => s.AppUser == user && gameModes.Contains(s.GameMode));
        if (stats != null && stats.Any())
        {
            foreach (var stat in stats)
            {
                this.dataContext.GameModeAppUserStats.Remove(stat);
            }
            await this.dataContext.SaveChangesAsync();
        }
    }
}