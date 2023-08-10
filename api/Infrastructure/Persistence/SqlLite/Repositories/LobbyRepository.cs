using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace SqlLite.Repositories;

public class LobbyRepository : ILobbyRepository
{
    private readonly IDataContext dataContext;
    private readonly IGameModesRepository gameModesRepository;

    public LobbyRepository(IDataContext dataContext, IGameModesRepository gameModesRepository)
    {
        this.dataContext = dataContext;
        this.gameModesRepository = gameModesRepository;
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
                    Username = user.UserName,
                    ConnectionState = ConnectionState.Connected
                }
            }
        };

        await this.dataContext.Lobbies.AddAsync(lobby);
        await this.dataContext.SaveChangesAsync();
        return lobby;
    }

    public async Task<Lobby?> DeleteLobbyAsync(AppUser user)
    {
        var lobby = await this.dataContext.Lobbies.FirstOrDefaultAsync(l => l.LobbyCreator == user);

        if (lobby != null)
        {
            this.dataContext.Lobbies.Remove(lobby);
            await this.dataContext.SaveChangesAsync();
        }

        return lobby;
    }

    public async Task<Lobby?> GetLobbyByIdAsync(int id) => await this.dataContext.Lobbies.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Lobby?> GetLobbyByLobbyNameAsync(string lobbyName) => await this.dataContext.Lobbies
                                                                                                  .Include(l => l.Connections)
                                                                                                  .Include(l => l.Users)
                                                                                                  .Include(l => l.LobbyCreator)
                                                                                                  .FirstOrDefaultAsync(l => l.LobbyName == lobbyName);

    public async Task<Lobby?> GetLobbyWithUserAsync(AppUser user) => await this.dataContext.Lobbies
                                                                                           .Include(l => l.CurrentGame)
                                                                                           .Include(l => l.CurrentGame.Stats)
                                                                                           .Include(l => l.Connections)
                                                                                           .Include(l => l.Users)
                                                                                           .Include(l => l.LobbyCreator)
                                                                                           .FirstOrDefaultAsync(l => l.Users.Any(u => u == user));

    public async Task<Lobby?> JoinLobbyAsync(AppUser user, string lobbyName)
    {
        Lobby? lobby = null;

        if (await this.CheckIsLobbyExistsAsync(lobbyName))
        {
            lobby = await this.GetLobbyByLobbyNameAsync(lobbyName);
            lobby.Users.Add(user);

            var connection = lobby.Connections.FirstOrDefault(c => c.Username == user.UserName);
            connection.ConnectionState = ConnectionState.Connected;

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
            lobby.Connections.Add(new Connection
            {
                Username = user.UserName,
                ConnectionId = connectionId,
                ConnectionState = ConnectionState.Pending
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
                                                     || l.Users.Any(c => c.UserName == user.UserName));

    public async Task<Lobby?> AddConnectionAsync(AppUser user, string lobbyName, string connectionId)
    {
        Lobby? lobby = null;

        if (await this.IsUserInLobbyAsync(user))
        {
            lobby = await this.GetLobbyByLobbyNameAsync(lobbyName);
            lobby?.Connections.Add(new Connection
            {
                Username = user.UserName,
                ConnectionId = connectionId,
                ConnectionState = ConnectionState.Connected
            });
            await this.dataContext.SaveChangesAsync();
        }

        return lobby;
    }

    public async Task<Lobby?> RemoveConnectionAsync(AppUser user, string lobbyName)
    {
        Lobby? lobby = null;

        if (await this.IsUserInLobbyAsync(user))
        {
            lobby = await this.GetLobbyByLobbyNameAsync(lobbyName);
            if (lobby != null)
            {
                lobby.Connections = lobby.Connections.Where(c => c.Username != user.UserName).ToList();
                await this.dataContext.SaveChangesAsync();
            }
        }

        return lobby;
    }

    public async Task CreateGameAsync(Lobby lobby, Game game)
    {
        lobby.CurrentGame = game;
        await this.dataContext.SaveChangesAsync();
    }

    public async Task FinishGameAsync(Lobby lobby)
    {
        lobby.CurrentGame.State = GameState.Finished;
        lobby.PreviousGames.Add(lobby.CurrentGame);
        lobby.CurrentGame = null;
        await this.dataContext.SaveChangesAsync();
    }

    public async Task RatePlayerAsync(Lobby lobby, AppUser player)
    {
        var stats = lobby.CurrentGame.Stats.LastOrDefault(s => s.AppUser == player);
        if (stats != null)
        {
            stats.Place = lobby.CurrentGame.Stats.Max(s => s.Place) + 1;
            await this.dataContext.SaveChangesAsync();
        }
    }
}