using Domain.Entities;

namespace Application.Interfaces;

public interface ILobbyRepository 
{
    Task<bool> IsUserInLobbyAsync(AppUser user);
    Task<Lobby?> GetLobbyByIdAsync(int id);
    Task<Lobby?> GetLobbyByLobbyNameAsync(string lobbyName);
    Task<Lobby?> GetLobbyWithUserAsync(AppUser user);
    Task<Lobby?> CreateLobbyAsync(AppUser user, string lobbyName, string connectionId);
    Task<Lobby?> RequestLobbyJoinAsync(AppUser user, string lobbyName, string connectionId);
    Task<Lobby?> AddConnectionAsync(AppUser user, string lobbyName, string connectionId);
    Task<Lobby?> RemoveConnectionAsync(AppUser user, string lobbyName);
    Task<Lobby?> JoinLobbyAsync(AppUser user, string lobbyName);
    Task<Lobby?> LeaveLobbyAsync(AppUser user, string connectionId);
    Task<Lobby?> DeleteLobbyAsync(AppUser user);
    Task CreateGameAsync(Lobby lobby, Game game);
    Task FinishGameAsync(Lobby lobby);
    Task RatePlayerAsync(Lobby lobby, AppUser player);
    Task RemoveCurrentGameAsync(Lobby lobby);
}