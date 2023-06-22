using Domain.Entities;
using Domain.Models;

namespace Application.Interfaces;

public interface IGameRepository
{
    Task AddGameAsync(Game game);
    Task<Game?> GetGameByIdAsync(int id);
    Task<Game?> GetGameWithPlayerAsync(AppUser player);
    Task UpdateGameStateAsync(Game game, GameState gameState);
    Task AddUserToStatsAsync(Game game, AppUser player);
}