using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Responses.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs;

//TODO: Extract all signalr string methods to constants

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
        var lobbyRequestMode = (LobbyConnectMode)Enum.Parse(typeof(LobbyConnectMode), lobbyRequestModeStringified);

        var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username)) return;

        var user = await this.userRepository.GetUserByUsernameAsync(username);
        if (user is null) return;

        var isUserInLobby = await this.lobbyRepository.IsUserInLobbyAsync(user);

        if ((lobbyRequestMode == LobbyConnectMode.Create || lobbyRequestMode == LobbyConnectMode.Join) && isUserInLobby) return;

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



    private async Task RequestJoinToLobbyAsync(string lobbyName, AppUser? user, Lobby? lobby)
    {
        if (lobby is null)
        {
            await Clients.Caller.SendAsync("LobbyWithNameIsNotExtist", lobbyName);
            return;
        }

        if (lobby.PendingConnections.Any(c => c.ConnectionId == Context.ConnectionId))
        {
            await Clients.Caller.SendAsync("JoinRequestSent", new LobbyDTO(lobby));
            return;
        }

        lobby = await this.lobbyRepository.RequestLobbyJoinAsync(user, lobbyName, Context.ConnectionId);
        await Clients.Caller.SendAsync("JoinRequestSent", new LobbyDTO(lobby));
        await Clients.Client(lobby.Connections.FirstOrDefault(c => c.Username == lobby.LobbyCreator.UserName).ConnectionId).SendAsync("JoinRequestReceived", new LobbyDTO(lobby));
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
        await Clients.Group(lobbyName).SendAsync("LobbyCreate", new LobbyDTO(lobby));
    }

    public async Task LeaveLobbyAsync(string username)
    {
        var user = await this.userRepository.GetUserByUsernameAsync(username);
        if (user is null)
        {
            await Clients.Caller.SendAsync("UserWithSuchNameDoesNotExist");
            return;
        }

        await LeaveLobbyAsync(user);
    }

    private async Task LeaveLobbyAsync(AppUser user)
    {
        var lobby = await this.lobbyRepository.GetLobbyWithUserAsync(user);
        if (lobby is null)
        {
            await Clients.Caller.SendAsync("NoLobbyWithSuchUser", user.UserName);
            return;
        }

        lobby = await this.lobbyRepository.LeaveLobbyAsync(user, Context.ConnectionId);
        await Clients.Caller.SendAsync("SuccessfulyLeftLobby");
        await Clients.Group(lobby.LobbyName).SendAsync("UserLeft", new LobbyDTO(lobby));
    }

    public async Task ApproveUserJoinAsync(string lobbyName, string approveUsername, bool isJoinApproved)
    {
        var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username))
        {
            await Clients.Caller.SendAsync("NoUserWasProvided");
            return;
        }

        var user = await this.userRepository.GetUserByUsernameAsync(username);
        if (user is null)
        {
            await Clients.Caller.SendAsync("NoSuchUser");
            return;
        }

        await ApproveUserJoinAsync(lobbyName, user, isJoinApproved, approveUsername);
    }

    private async Task ApproveUserJoinAsync(string lobbyName, AppUser user, bool isJoinApproved, string approveUsername)
    {
        var lobby = await this.lobbyRepository.GetLobbyByLobbyNameAsync(lobbyName);
        if (lobby is null)
        {
            await Clients.Caller.SendAsync("LobbyWithNameIsNotExtist", lobbyName);
            return;
        }

        if (lobby.LobbyCreator != user)
        {
            await Clients.Caller.SendAsync("NoPermisionToApproveJoin");
            return;
        }

        if (lobby.PendingConnections.Any(c => c.Username != approveUsername))
        {
            await Clients.Caller.SendAsync("NoSuchPendingJoinRequestWithProvidedName", approveUsername);
            return;
        }

        var approveUser = await this.userRepository.GetUserByUsernameAsync(approveUsername);

        if (approveUser is null)
        {
            await Clients.Caller.SendAsync("NoSuchUser", approveUser);
            return;
        }

        lobby = await this.lobbyRepository.JoinLobbyAsync(approveUser, lobbyName);
        await Clients.Group(lobbyName).SendAsync("UserJoined", new LobbyDTO(lobby));
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
    }
}