using Domain.Entities;

namespace Application.Interfaces;

public interface IGameModesRepository
{
    Task<GameMode?> GetGameModeByIdAsync(int id);
    Task<GameMode?> GetGameModeByNameAsync(string gameModeName);
    Task<List<GameMode>> GetGameModesAsync();
    Task<List<GameMode>> GetGameModesAsync(int start, int count);
    Task ChangeGameInLobbyAsync(Lobby lobby, GameMode gameMode);
}