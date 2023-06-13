using Application.Extensions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Constants;
using Domain.Responses.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs;

[Authorize]
public class LobbyHub : Hub
{
    private readonly IUserRepository userRepository;
    private readonly ILobbyRepository lobbyRepository;
    private readonly IGameModesRepository gameModesRepository;

    public LobbyHub(IUserRepository userRepository, ILobbyRepository lobbyRepository, IGameModesRepository gameModeRepository)
    {
        this.userRepository = userRepository;
        this.lobbyRepository = lobbyRepository;
        this.gameModesRepository = gameModeRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is null)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.HttpContextMustBeProvided);
            return;
        }

        var lobbyRequestModeStringified = httpContext.Request.Query["lobbyConnectMode"].ToString();
        if (string.IsNullOrEmpty(lobbyRequestModeStringified))
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.LobbyConnectModeMustBeProvidedInQuery);
            return;
        }
        var lobbyRequestMode = (LobbyConnectMode)Enum.Parse(typeof(LobbyConnectMode), lobbyRequestModeStringified);

        var user = await HubExtensions.GetCallerAsAppUserAsync(Context, Clients, this.userRepository);
        if (user is null) return;

        var isUserInLobby = await this.lobbyRepository.IsUserInLobbyAsync(user);

        if (lobbyRequestMode == LobbyConnectMode.None)
        {
            if (isUserInLobby)
            {
                var lobby = await this.lobbyRepository.GetLobbyWithUserAsync(user);
                if (lobby is not null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, lobby.LobbyName);
                    await this.lobbyRepository.AddConnectionAsync(user, lobby.LobbyName, Context.ConnectionId);
                    await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.LobbyForUserFound, new LobbyDTO(lobby));
                    return;
                }
            }

            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.LobbyForUserNotFound);
            return;
        }
        else
        {
            if ((lobbyRequestMode == LobbyConnectMode.Create || lobbyRequestMode == LobbyConnectMode.Join) && isUserInLobby)
            {
                await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.UserAlreadyInLobby);
                return;
            }

            var lobbyName = httpContext.Request.Query["lobbyName"].ToString();
            if (string.IsNullOrEmpty(lobbyName))
            {
                await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.LobbyNameMustBeProvidedInQuery);
                return;
            }

            var lobby = await this.lobbyRepository.GetLobbyByLobbyNameAsync(lobbyName);

            switch (lobbyRequestMode)
            {
                case LobbyConnectMode.Create:
                    await CreateLobbyAsync(user, lobbyName, lobby);
                    return;
                case LobbyConnectMode.Join:
                    await RequestJoinToLobbyAsync(lobbyName, user, lobby);
                    return;
            }
        }
    }

    private async Task RequestJoinToLobbyAsync(string lobbyName, AppUser? user, Lobby? lobby)
    {
        if (lobby is null)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.NoLobbyWithSuchName, lobbyName);
            return;
        }

        if (lobby.Connections.Where(c => c.ConnectionState == ConnectionState.Pending).Any(c => c.ConnectionId == Context.ConnectionId))
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.JoinRequestSent);
            return;
        }

        lobby = await this.lobbyRepository.RequestLobbyJoinAsync(user, lobbyName, Context.ConnectionId);
        await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.JoinRequestSent);
        var lobbyLeaderConnectionId = lobby?.Connections.FirstOrDefault(c => c.Username == lobby.LobbyCreator.UserName)?.ConnectionId;
        await Clients.Clients(lobbyLeaderConnectionId)
                     .SendAsync(LobbyHubMethodNameConstants.JoinRequestReceived, new LobbyDTO(lobby));
    }

    private async Task CreateLobbyAsync(AppUser user, string lobbyName, Lobby? lobby)
    {
        if (lobby is not null)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.LobbyAlreadyExists, lobbyName);
            return;
        }

        lobby = await this.lobbyRepository.CreateLobbyAsync(user, lobbyName, Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
        await Clients.Group(lobbyName).SendAsync(LobbyHubMethodNameConstants.LobbyCreate, new LobbyDTO(lobby));
    }

    public async Task LeaveLobbyAsync(string username)
    {
        var user = await this.userRepository.GetUserByUsernameAsync(username);
        if (user is null)
        {
            await Clients.Caller.SendAsync(CommonHubMethodNameConstants.NoUserWithSuchName);
            return;
        }

        await LeaveLobbyAsync(user);
    }

    private async Task<Lobby?> LeaveLobbyAsync(AppUser user)
    {
        var lobby = await this.lobbyRepository.GetLobbyWithUserAsync(user);
        if (lobby is null)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.NoLobbyWithSuchUser, user.UserName);
            return lobby;
        }

        lobby = await this.lobbyRepository.LeaveLobbyAsync(user, Context.ConnectionId);
        await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.SuccessfulyLeftLobby);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobby.LobbyName);
        await Clients.Group(lobby.LobbyName).SendAsync(LobbyHubMethodNameConstants.UserLeftLobby, new LobbyDTO(lobby));
        return lobby;
    }

    public async Task ApproveUserJoinAsync(string lobbyName, string approveUsername, bool isJoinApproved)
    {
        var user = await HubExtensions.GetCallerAsAppUserAsync(Context, Clients, this.userRepository);
        if (user is null)
        {
            return;
        }
        await ApproveUserJoinAsync(lobbyName, user, isJoinApproved, approveUsername);
    }

    private async Task<(AppUser?, Lobby?)> GetCallerAsAppUserOwnerLobbyAsync()
    {
        var user = await HubExtensions.GetCallerAsAppUserAsync(Context, Clients, this.userRepository);
        if (user is null)
        {
            return (null, null);
        }
        var lobby = await this.lobbyRepository.GetLobbyWithUserAsync(user);
        if (lobby is null)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.NoLobbyWithSuchUser);
            return (null, null);
        }
        if (lobby.LobbyCreator != user)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.NoPermisionOwnerLobby);
            return (null, null);
        }
        return (user, lobby);
    }

    private async Task ApproveUserJoinAsync(string lobbyName, AppUser user, bool isJoinApproved, string approveUsername)
    {
        var lobby = await this.lobbyRepository.GetLobbyByLobbyNameAsync(lobbyName);
        if (lobby is null)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.NoLobbyWithSuchName, lobbyName);
            return;
        }

        if (lobby.LobbyCreator != user)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.NoPermisionToApproveJoin);
            return;
        }

        if (lobby.Connections.Where(c => c.ConnectionState == ConnectionState.Pending).Any(c => c.Username != approveUsername))
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.NoSuchPendingJoinRequestWithProvidedName, approveUsername);
            return;
        }

        var approveUser = await this.userRepository.GetUserByUsernameAsync(approveUsername);

        if (approveUser is null)
        {
            await Clients.Caller.SendAsync(CommonHubMethodNameConstants.NoUserWithSuchName, approveUser);
            return;
        }

        var pendingConnection = lobby.Connections.Where(c => c.ConnectionState == ConnectionState.Pending).FirstOrDefault(pc => pc.Username == approveUsername)?.ConnectionId;

        if (isJoinApproved)
        {
            await Groups.AddToGroupAsync(pendingConnection, lobbyName);
            lobby = await this.lobbyRepository.JoinLobbyAsync(approveUser, lobbyName);
            await Clients.Group(lobbyName).SendAsync(LobbyHubMethodNameConstants.UserJoined, new LobbyDTO(lobby));
        }
        else
        {
            lobby = await this.lobbyRepository.RemoveConnectionAsync(approveUser, lobbyName);
            await Clients.Clients(pendingConnection).SendAsync(LobbyHubMethodNameConstants.UserJoinDenied);
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.PendingConnectionRemoved, new LobbyDTO(lobby));
        }
    }

    public async Task DeleteLobbyAsync(string lobbyName)
    {
        var user = await HubExtensions.GetCallerAsAppUserAsync(Context, Clients, this.userRepository);
        if (user is null)
        {
            return;
        }
        var lobby = await this.lobbyRepository.DeleteLobbyAsync(user);
        await Clients.Group(lobbyName).SendAsync(LobbyHubMethodNameConstants.LobbyWasDeleted);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyName);
    }

    public async Task KickUserFromLobbyAsync(string usernameKick)
    {
        var (user, lobby) = await GetCallerAsAppUserOwnerLobbyAsync();
        if (user is null)
        {
            return;
        }

        var userToKick = lobby!.Users.FirstOrDefault(u => string.Equals(u.UserName, usernameKick));
        if (userToKick is null)
        {
            await Clients.Caller.SendAsync(CommonHubMethodNameConstants.NoUserWithSuchName);
            return;
        }

        string connectionKickedUser = lobby.Connections.First(pc => pc.Username == userToKick.UserName).ConnectionId;
        await this.lobbyRepository.LeaveLobbyAsync(userToKick, connectionKickedUser);

        await Clients.Clients(connectionKickedUser).SendAsync(LobbyHubMethodNameConstants.UHaveBeenKicked);
        await Clients.Group(lobby.LobbyName).SendAsync(LobbyHubMethodNameConstants.UserKicked, new LobbyDTO(lobby));
    }

    public async Task ChangeGameModeAsync(string gameModeName)
    {
        var (user, lobby) = await GetCallerAsAppUserOwnerLobbyAsync();
        if (user is null || lobby is null)
        {
            return;
        }

        var gameMode = await this.gameModesRepository.GetGameModeByNameAsync(gameModeName);
        if (gameMode is null)
        {
            await Clients.Caller.SendAsync(LobbyHubMethodNameConstants.NoSuchGameModeWithProvidedName);
            return;
        }

        await gameModesRepository.ChangeGameInLobbyAsync(lobby, gameMode);
        await Clients.Group(lobby.LobbyName).SendAsync(LobbyHubMethodNameConstants.LobbyGameModeChanged, new LobbyDTO(lobby, gameMode));
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var user = await HubExtensions.GetCallerAsAppUserAsync(Context, Clients, this.userRepository);
        if (user is not null)
        {
            var lobby = await this.lobbyRepository.GetLobbyWithUserAsync(user);
            if (lobby is not null)
            {
                await this.lobbyRepository.RemoveConnectionAsync(user, lobby.LobbyName);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobby.LobbyName);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
}