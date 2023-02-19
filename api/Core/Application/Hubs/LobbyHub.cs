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

        switch (lobbyRequestMode)
        {
            case LobbyRequestMode.Create:
                var lobby = await this.lobbyRepository.GetLobbyByLobbyNameAsync(lobbyName);
                if (lobby is not null)
                {
                    await Clients.Caller.SendAsync("LobbyWithThisNameExists", lobbyName);
                }
                lobby = await CreateLobbyAsync(user, lobbyName);
                await Clients.Caller.SendAsync("LobbyCreate", lobby);
                break;
        }
    }

    private async Task<Lobby?> CreateLobbyAsync(AppUser user, string lobbyName)
    {
        var lobby = await this.lobbyRepository.CreateLobbyAsync(user, lobbyName, Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
        await Clients.Group(lobbyName).SendAsync("LobbyCreate", lobby);
        return lobby;
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
    }
}