using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace SqlLite.Repositories;

public class GameRepository : IGameRepository
{
    private readonly IDataContext dataContext;

    public GameRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task<GameMode?> GetGameByIdAsync(int id) => await this.dataContext.Games.FirstOrDefaultAsync(g => g.Id == id);

    public async Task<List<GameMode>> GetGamesAsync() => await this.dataContext.Games.ToListAsync();

    public async Task<List<GameMode>> GetGamesAsync(int start, int count)
    {
        var gamesCount = this.dataContext.Games.Count();
        start = start.ToStart(gamesCount);
        return await this.dataContext.Games.Skip(start).Take(count.ToCount(start, gamesCount)).ToListAsync();
    }
}