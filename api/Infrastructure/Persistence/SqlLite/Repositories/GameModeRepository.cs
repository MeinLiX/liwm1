using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace SqlLite.Repositories;

public class GameModeRepository : IGameModesRepository
{
    private readonly IDataContext dataContext;

    public GameModeRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task<GameMode?> GetGameModeByIdAsync(int id) => await this.dataContext.GameModes.Include(gm => gm.Lobbies).AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);

    public async Task<GameMode?> GetGameModeByNameAsync(string gameModeName) => await this.dataContext.GameModes.Include(gm => gm.Lobbies).AsNoTracking().FirstOrDefaultAsync(gm => gm.Name == gameModeName);

    public async Task<List<GameMode>> GetGameModesAsync() => await this.dataContext.GameModes.Include(gm => gm.Lobbies).AsNoTracking().ToListAsync();
    
    public async Task<List<GameMode>> GetGameModesAsync(int start, int count)
    {
        var gamesCount = this.dataContext.GameModes.Count();
        start = start.ToStart(gamesCount);
        return await this.dataContext.GameModes.Include(gm => gm.Lobbies).Skip(start).Take(count.ToCount(start, gamesCount)).AsNoTracking().ToListAsync();
    }
}