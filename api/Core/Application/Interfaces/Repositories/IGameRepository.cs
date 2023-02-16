using Domain.Entities;

namespace Application.Interfaces;

public interface IGameRepository
{
    Task<GameMode?> GetGameByIdAsync(int id);
    Task<List<GameMode>> GetGamesAsync();
    Task<List<GameMode>> GetGamesAsync(int start, int count);
}