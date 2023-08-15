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

    public async Task AddUserToStatsAsync(Game game, AppUser player)
    {
        game.Stats.Add(new GameAppUsersStats
        {
            AppUser = player,
            Game = game
        });

        await this.dataContext.SaveChangesAsync();
    }

    public async Task<Game?> GetGameByIdAsync(int id) => await this.dataContext.Games.FirstOrDefaultAsync(g => g.Id == id);

    public async Task<Game?> GetGameWithPlayerAsync(AppUser player) => await this.dataContext.Games.Include(g => g.Stats).OrderBy(g => g.CreatedAt).LastOrDefaultAsync(g => g.Stats.Any(s => s.AppUser == player) && g.State != GameState.Finished);

    public async Task UpdateGameStateAsync(Game game, GameState gameState)
    {
        game.State = gameState;
        await this.dataContext.SaveChangesAsync();
    }
}