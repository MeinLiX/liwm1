using Domain.Entities;

namespace Application.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetGameByIdAsync(int id);
    Task<List<Game>> GetGamesAsync();
    Task<List<Game>> GetGamesAsync(int start, int count);
}