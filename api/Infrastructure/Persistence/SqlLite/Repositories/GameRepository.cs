using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace SqlLite.Repositories;

public class GameModeRepository : IGameModeRepository
{
    private readonly IDataContext dataContext;

    public GameModeRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task<GameMode?> GetGameByIdAsync(int id) => await this.dataContext.GameModes.FirstOrDefaultAsync(g => g.Id == id);

    public async Task<List<GameMode>> GetGamesAsync() => await this.dataContext.GameModes.ToListAsync();

    public async Task<List<GameMode>> GetGamesAsync(int start, int count)
    {
        var gamesCount = this.dataContext.GameModes.Count();
        start = start.ToStart(gamesCount);
        return await this.dataContext.GameModes.Skip(start).Take(count.ToCount(start, gamesCount)).ToListAsync();
    }
}