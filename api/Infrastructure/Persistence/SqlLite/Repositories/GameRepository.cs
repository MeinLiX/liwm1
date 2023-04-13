using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SqlLite.Repositories;

public class GameRepository : IGameRepository
{
    private readonly IDataContext dataContext;

    public GameRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task AddGameAsync(Game game)
    {
        await this.dataContext.Games.AddAsync(game);
        await this.dataContext.SaveChangesAsync();
    }

    public async Task<Game?> GetGameByIdAsync(int id) => await this.dataContext.Games.FirstOrDefaultAsync(g => g.Id == id);
}