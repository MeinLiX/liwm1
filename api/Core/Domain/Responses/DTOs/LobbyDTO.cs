using Domain.Entities;
using Domain.Models;

namespace Domain.Responses.DTOs;

public class LobbyDTO
{
    public string LobbyName { get; }
    public UserDetailDTO LobbyCreator { get; }
    public GameModeDTO GameMode { get; }
    public GameDTO CurrentGame { get; set; }
    public ICollection<UserDetailDTO> Users { get; }
    public ICollection<string> PendingConnections { get; }
    public ICollection<GameDTO> PreviousGames { get; set; }

    public LobbyDTO(Lobby lobby)
    {
        this.LobbyName = lobby.LobbyName;
        this.LobbyCreator = new(lobby.LobbyCreator);
        this.Users = new List<UserDetailDTO>(lobby.Users.Select(u => new UserDetailDTO(u)));
        this.PendingConnections = new List<string>(lobby.Connections.Where(c => c.ConnectionState == ConnectionState.Pending).Select(c => c.Username));
        this.PreviousGames = new List<GameDTO>(lobby.PreviousGames.Select(g => new GameDTO(g)));
    }

    public LobbyDTO(Lobby lobby, GameMode gameMode)
    {
        this.LobbyName = lobby.LobbyName;
        this.LobbyCreator = new(lobby.LobbyCreator);
        this.Users = new List<UserDetailDTO>(lobby.Users.Select(u => new UserDetailDTO(u)));
        this.PendingConnections = new List<string>(lobby.Connections.Where(c => c.ConnectionState == ConnectionState.Pending).Select(c => c.Username));
        this.PreviousGames = new List<GameDTO>(lobby.PreviousGames.Select(g => new GameDTO(g)));
        this.GameMode = new GameModeDTO(gameMode);
    }

    public LobbyDTO(Lobby lobby, GameMode gameMode, Game game)
    {
        this.LobbyName = lobby.LobbyName;
        this.LobbyCreator = new(lobby.LobbyCreator);
        this.Users = new List<UserDetailDTO>(lobby.Users.Select(u => new UserDetailDTO(u)));
        this.PendingConnections = new List<string>(lobby.Connections.Where(c => c.ConnectionState == ConnectionState.Pending).Select(c => c.Username));
        this.PreviousGames = new List<GameDTO>(lobby.PreviousGames?.Select(g => new GameDTO(g)));
        this.GameMode = new GameModeDTO(gameMode);
        this.CurrentGame = new GameDTO(game);
    }
}