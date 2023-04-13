using Domain.Entities;

namespace Application.Interfaces;

public interface IGameRepository
{
    Task AddGameAsync(Game game);
    Task<Game?> GetGameByIdAsync(int id);
}