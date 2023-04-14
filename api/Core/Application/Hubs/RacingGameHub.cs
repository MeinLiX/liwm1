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
        var user = await HubExtensions.GetCallerAsAppUserAsync(Context, Clients, this.userRepository);
        if (user is null)
        {
            return;
        }

        var car = await this.racingCarRepository.GetRacingCarByRacerNameAsync(user.UserName);
        if (car is null)
        {
            car = new RacingCar
            {
                RacerName = user.UserName
            };
            await this.racingCarRepository.AddRacingCarAsync(car);
        }
        await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.RecieveRacingCar, car);
        //TODO: Add group creating | adding. Add sending new car to other players 
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        //TODO: Add car deleting and group leaving
        await base.OnDisconnectedAsync(exception);
    }
}