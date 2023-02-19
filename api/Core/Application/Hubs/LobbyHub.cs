using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs;

[Authorize]
public class LobbyHub : Hub
{
    private readonly IUserRepository userRepository;
    private readonly ILobbyRepository lobbyRepository;

    public LobbyHub(IUserRepository userRepository, ILobbyRepository lobbyRepository)
    {
        this.userRepository = userRepository;
        this.lobbyRepository = lobbyRepository;
    }

    public override async Task OnConnectedAsync()
    {
        //TODO: Add returning check explanations to the client

        var httpContext = Context.GetHttpContext();
        if (httpContext is null) return;

        var lobbyName = httpContext.Request.Query["lobbyName"].ToString();
        if (string.IsNullOrEmpty(lobbyName)) return;

        var lobbyRequestModeStringified = httpContext.Request.Query["lobbyRequestMode"].ToString();
        if (string.IsNullOrEmpty(lobbyRequestModeStringified)) return;
        var lobbyRequestMode = (LobbyRequestMode)Enum.Parse(typeof(LobbyRequestMode), lobbyRequestModeStringified);

        var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username)) return;

        var user = await this.userRepository.GetUserByUsernameAsync(username);
        if (user is null) return;

        var isUserInLobby = await this.lobbyRepository.IsUserInLobbyAsync(user);

        if ((lobbyRequestMode == LobbyRequestMode.Create || lobbyRequestMode == LobbyRequestMode.Join) && isUserInLobby) return;

        if ((lobbyRequestMode == LobbyRequestMode.Delete || lobbyRequestMode == LobbyRequestMode.ApproveJoin || lobbyRequestMode == LobbyRequestMode.Leave) && !isUserInLobby) return;

        var lobby = await this.lobbyRepository.GetLobbyByLobbyNameAsync(lobbyName);

        switch (lobbyRequestMode)
        {
            case LobbyRequestMode.Create:
                await CreateLobbyAsync(user, lobbyName, lobby);
                return;
            case LobbyRequestMode.Join:
                await RequestJoinToLobbyAsync(lobbyName, user, lobby);
                return;
            case LobbyRequestMode.Leave:
                await LeaveLobbyAsync(lobbyName, user, lobby);
                return;
        }
    }

    private async Task LeaveLobbyAsync(string lobbyName, AppUser? user, Lobby? lobby)
    {
        if (lobby is null)
        {
            await Clients.Caller.SendAsync("NoLobbyWithSuchUser", user.UserName);
            return;
        }

        lobby = await this.lobbyRepository.LeaveLobbyAsync(user, Context.ConnectionId);
        await Clients.Caller.SendAsync("SuccessfulyLeftLobby");
        await Clients.Group(lobbyName).SendAsync("UserLeft", lobby);
    }

    private async Task RequestJoinToLobbyAsync(string lobbyName, AppUser? user, Lobby? lobby)
    {
        if (lobby is null)
        {
            await Clients.Caller.SendAsync("LobbyWithNameIsNotExtist", lobbyName);
            return;
        }

        if (lobby.PendingConnections.Any(c => c.ConnectionId == Context.ConnectionId))
        {
            await Clients.Caller.SendAsync("JoinRequestSent", lobby);
            return;
        }

        lobby = await this.lobbyRepository.RequestLobbyJoinAsync(user, lobbyName, Context.ConnectionId);
        await Clients.Caller.SendAsync("JoinRequestSent", lobby);
        await Clients.Client(lobby.Connections.FirstOrDefault(c => c.Username == lobby.LobbyCreator.UserName).ConnectionId).SendAsync("JoinRequestReceived", lobby);
    }

    private async Task CreateLobbyAsync(AppUser user, string lobbyName, Lobby? lobby)
    {
        if (lobby is not null)
        {
            await Clients.Caller.SendAsync("LobbyWithThisNameExists", lobbyName);
            return;
        }

        lobby = await this.lobbyRepository.CreateLobbyAsync(user, lobbyName, Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
        await Clients.Group(lobbyName).SendAsync("LobbyCreate", lobby);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
    }
}