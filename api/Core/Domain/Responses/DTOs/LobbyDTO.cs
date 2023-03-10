using Domain.Entities;
using Domain.Models;

namespace Domain.Responses.DTOs;

public class LobbyDTO
{
    public string LobbyName { get; }
    public UserDetailDTO LobbyCreator { get; }
    public GameModeDTO GameModeDTO { get; }
    public ICollection<UserDetailDTO> Users { get; }
    public ICollection<string> PendingConnections { get; }

    public LobbyDTO(Lobby lobby)
    {
        this.LobbyName = lobby.LobbyName;
        this.LobbyCreator = new(lobby.LobbyCreator);
        this.Users = new List<UserDetailDTO>(lobby.Users.Select(u => new UserDetailDTO(u)));
        this.PendingConnections = new List<string>(lobby.Connections.Where(c => c.ConnectionState == ConnectionState.Pending).Select(c => c.Username));
        // this.GameModeDTO = new GameModeDTO(lobby.GameMode);
    }
}