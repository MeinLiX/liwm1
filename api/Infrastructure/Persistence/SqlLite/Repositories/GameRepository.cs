using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
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

    public async Task<Game?> GetGameWithPlayerAsync(AppUser player) => await this.dataContext.Games.LastOrDefaultAsync(g => g.Players.Contains(player) && g.GameState != GameState.Finished);

    public async Task UpdateGameStateAsync(Game game, GameState gameState)
    {
        game.GameState = gameState;
        await this.dataContext.SaveChangesAsync();
    }
}