using Domain.Entities;

namespace Application.Interfaces;

public interface IGameModesRepository
{
    Task<GameMode?> GetGameByIdAsync(int id);
    Task<GameMode?> GetGameByNameAsync(string gameModeName);
    Task<List<GameMode>> GetGamesAsync();
    Task<List<GameMode>> GetGamesAsync(int start, int count);
}