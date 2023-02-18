using Domain.Entities;

namespace Application.Interfaces;

public interface ILobbyRepository 
{
    Task<Lobby?> GetLobbyByIdAsync(int id);
    Task<Lobby?> GetLobbyByLobbyNameAsync(string lobbyName);
    Task<Lobby?> GetLobbyWithUserAsync(AppUser user);
    Task<Lobby?> CreateLobbyAsync(AppUser user, string lobbyName);
    Task<Lobby?> JoinLobbyAsync(AppUser user, string lobbyName);
    Task<Lobby?> LeaveLobbyAsync(AppUser user);
    Task<Lobby?> DeleteLobbyAsync(AppUser user);
}