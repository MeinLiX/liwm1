using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs;

[Authorize]
public class LobbyHub : Hub
{
    private readonly ILobbyRepository lobbyRepository;

    public LobbyHub(ILobbyRepository lobbyRepository)
    {
        this.lobbyRepository = lobbyRepository;
    }

    public override async Task OnConnectedAsync()
    {
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
    }
}