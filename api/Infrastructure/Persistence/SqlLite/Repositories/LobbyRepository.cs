using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SqlLite.Repositories;

public class LobbyRepository : ILobbyRepository
{
    private readonly IDataContext dataContext;

    public LobbyRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task<Lobby?> CreateLobbyAsync(AppUser user, string lobbyName, string connectionId)
    {
        if (await this.CheckIsLobbyExistsAsync(lobbyName))
        {
            return null;
        }

        var lobby = new Lobby
        {
            Users = new List<AppUser>()
            {
                user
            },
            LobbyCreator = user,
            LobbyName = lobbyName,
            Connections = new List<Connection>()
            {
                new Connection
                {
                    ConnectionId = connectionId,
                    Username = user.UserName
                }
            }
        };

        await this.dataContext.Lobbies.AddAsync(lobby);
        await this.dataContext.SaveChangesAsync();
        return lobby;
    }

    public async Task<Lobby?> DeleteLobbyAsync(AppUser user)
    {
        var lobby = await this.GetLobbyWithUserAsync(user);

        if (lobby != null && lobby.LobbyCreator == user)
        {
            this.dataContext.Lobbies.Remove(lobby);
            await this.dataContext.SaveChangesAsync();
        }

        return lobby;
    }

    public async Task<Lobby?> GetLobbyByIdAsync(int id) => await this.dataContext.Lobbies.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Lobby?> GetLobbyByLobbyNameAsync(string lobbyName) => await this.dataContext.Lobbies.FirstOrDefaultAsync(l => l.LobbyName == lobbyName);

    public async Task<Lobby?> GetLobbyWithUserAsync(AppUser user) => await this.dataContext.Lobbies.FirstOrDefaultAsync(l => l.Users.Any(u => u == user));

    public async Task<Lobby?> JoinLobbyAsync(AppUser user, string lobbyName)
    {
        Lobby? lobby = null;

        if (await this.CheckIsLobbyExistsAsync(lobbyName))
        {
            lobby = await this.GetLobbyByLobbyNameAsync(lobbyName);
            lobby.Users.Add(user);

            var connection = lobby.PendingConnections.FirstOrDefault(c => c.Username == user.UserName);

            lobby.Connections.Add(connection);
            lobby.PendingConnections.Remove(connection);

            await this.dataContext.SaveChangesAsync();
        }

        return lobby;
    }

    public async Task<Lobby?> RequestLobbyJoinAsync(AppUser user, string lobbyName, string connectionId)
    {
        Lobby? lobby = null;

        if (await this.CheckIsLobbyExistsAsync(lobbyName))
        {
            lobby = await this.GetLobbyByLobbyNameAsync(lobbyName);
            lobby.PendingConnections.Add(new Connection
            {
                Username = user.UserName,
                ConnectionId = connectionId
            });
            await this.dataContext.SaveChangesAsync();
        }

        return lobby;
    }

    public async Task<Lobby?> LeaveLobbyAsync(AppUser user, string connectionId)
    {
        Lobby? lobby = null;

        if (await this.dataContext.Lobbies.AnyAsync(l => l.Users.Any(u => u == user)))
        {
            lobby = await this.dataContext.Lobbies.FirstOrDefaultAsync(l => l.Users.Any(u => u == user));
            lobby.Users.Remove(user);
            lobby.Connections = lobby.Connections.Where(c => c.ConnectionId != connectionId).ToList();
            await this.dataContext.SaveChangesAsync();
        }

        return lobby;
    }

    private async Task<bool> CheckIsLobbyExistsAsync(string lobbyName)
        => await this.dataContext.Lobbies.AnyAsync(l => l.LobbyName == lobbyName);

    public async Task<bool> IsUserInLobbyAsync(AppUser user)
        => await this.dataContext.Lobbies.AnyAsync(l => l.Connections.Any(c => c.Username == user.UserName)
                                                     || l.PendingConnections.Any(c => c.Username == user.UserName));
}