using Application.Extensions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs;

[Authorize]
public class RacingGameHub : Hub
{
    private readonly IUserRepository userRepository;
    private readonly ILobbyRepository lobbyRepository;
    private readonly IRacingCarRepository racingCarRepository;

    public RacingGameHub(IUserRepository userRepository, ILobbyRepository lobbyRepository, IRacingCarRepository racingCarRepository)
    {
        this.userRepository = userRepository;
        this.lobbyRepository = lobbyRepository;
        this.racingCarRepository = racingCarRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var userWithLobby = await GetUserWithLobbyAsync();
        if (userWithLobby.Item1 is not null && userWithLobby.Item2 is not null)
        {
            var car = await this.racingCarRepository.GetRacingCarByRacerNameAsync(userWithLobby.Item1.UserName);
            if (car is null)
            {
                car = new RacingCar
                {
                    RacerName = userWithLobby.Item1.UserName
                };
                await this.racingCarRepository.AddRacingCarAsync(car);
            }

            await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.CarCreated, car);
            await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.RecievedNewRacingCar, car);
            await Groups.AddToGroupAsync(Context.ConnectionId, userWithLobby.Item2.LobbyName);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userWithLobby = await GetUserWithLobbyAsync();
        if (userWithLobby.Item1 is not null && userWithLobby.Item2 is not null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userWithLobby.Item2.LobbyName);

            var car = await this.racingCarRepository.GetRacingCarByRacerNameAsync(userWithLobby.Item1.UserName);
            if (car is not null)
            {
                await this.racingCarRepository.DeleteRacingCarByIdAsync(car.Id);
                await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.CarHasBeenDeleted);
                await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.OtherCarWithIdHasBeenDeleted, car.Id);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task<(AppUser?, Lobby?)> GetUserWithLobbyAsync()
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

        return (user, lobby);
    }
}