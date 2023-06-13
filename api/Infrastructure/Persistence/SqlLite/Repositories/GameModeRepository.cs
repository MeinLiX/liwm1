using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace SqlLite.Repositories;

public class GameModesRepository : IGameModesRepository
{
    private readonly IDataContext dataContext;

    public GameModesRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task ChangeGameInLobbyAsync(Lobby lobby, GameMode gameMode)
    {
        gameMode.Lobbies.Add(lobby);
        await this.dataContext.SaveChangesAsync();
    }

    public async Task<GameMode?> GetGameModeByIdAsync(int id) => await this.dataContext.GameModes.Include(gm => gm.Lobbies).FirstOrDefaultAsync(g => g.Id == id);

    public async Task<GameMode?> GetGameModeByNameAsync(string gameModeName) => await this.dataContext.GameModes.Include(gm => gm.Lobbies).FirstOrDefaultAsync(gm => gm.Name == gameModeName);

    public async Task<List<GameMode>> GetGameModesAsync() => await this.dataContext.GameModes.Include(gm => gm.Lobbies).ToListAsync();
    
    public async Task<List<GameMode>> GetGameModesAsync(int start, int count)
    {
        var gamesCount = this.dataContext.GameModes.Count();
        start = start.ToStart(gamesCount);
        return await this.dataContext.GameModes.Include(gm => gm.Lobbies).Skip(start).Take(count.ToCount(start, gamesCount)).ToListAsync();
    }
}