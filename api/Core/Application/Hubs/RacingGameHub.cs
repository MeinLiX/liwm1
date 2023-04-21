using Application.Extensions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
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
    private readonly IGameRepository gameRepository;
    private readonly IGameModesRepository gameModesRepository;

    public RacingGameHub(IUserRepository userRepository, ILobbyRepository lobbyRepository, IRacingCarRepository racingCarRepository, IGameRepository gameRepository, IGameModesRepository gameModesRepository)
    {
        this.userRepository = userRepository;
        this.lobbyRepository = lobbyRepository;
        this.racingCarRepository = racingCarRepository;
        this.gameRepository = gameRepository;
        this.gameModesRepository = gameModesRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var userWithLobby = await GetUserWithLobbyAsync();
        if (userWithLobby.Item1 is not null && userWithLobby.Item2 is not null)
        {
            if (userWithLobby.Item2.CurrentGame is null)
            {
                var gameMode = await this.gameModesRepository.GetGameModeByNameAsync("Racing");

                var game = new Game
                {
                    GameState = GameState.Created,
                    Players = new List<AppUser>
                    {
                        userWithLobby.Item1
                    },
                    GameMode = gameMode
                };
                await this.lobbyRepository.CreateGameAsync(userWithLobby.Item2, game);
            }
            else
            {
                if (userWithLobby.Item2.CurrentGame.GameState == GameState.Created)
                {
                    await this.lobbyRepository.AddPlayerToLobbyGame(userWithLobby.Item2, userWithLobby.Item1);
                }
                else
                {
                    await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.GameAlreadyStarted);
                    await Groups.AddToGroupAsync(Context.ConnectionId, userWithLobby.Item2.LobbyName);
                    return;
                }
            }

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

    public async Task SetReadyToCarAsync(int carId, bool isReady)
    {
        var userWithLobby = await GetUserWithLobbyAsync();
        if (userWithLobby.Item1 is not null && userWithLobby.Item2 is not null)
        {
            var car = await this.racingCarRepository.GetRacingCarByIdAsync(carId);
            if (car is null)
            {
                await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.NoSuchCar);
                return;
            }

            car.IsReady = isReady;
            await this.racingCarRepository.SaveChangesAsync();

            await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.CarReadyStateUpdated, car);

            var game = await this.gameRepository.GetGameWithPlayerAsync(userWithLobby.Item1);
            if (game is not null)
            {
                var cars = await Task.WhenAll(game.Players.Where(p => p.UserName != userWithLobby.Item1.UserName)
                                                          .Select(async p =>
                                                                    await this.racingCarRepository.GetRacingCarByRacerNameAsync(p.UserName)));
                if (cars.All(c => c?.IsReady ?? false))
                {
                    await gameRepository.UpdateGameStateAsync(game, GameState.Started);
                    await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.GameStarting);
                }
            }
        }
    }

    public async Task BoostCarAsync(int carId, RacingCarBoostMode racingCarBoostMode)
    {
        var userWithLobby = await GetUserWithLobbyAsync();
        if (userWithLobby.Item1 is not null && userWithLobby.Item2 is not null)
        {
            var car = await this.racingCarRepository.GetRacingCarByIdAsync(carId);
            if (car is null)
            {
                await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.NoSuchCar);
                return;
            }

            car.RacingCarBoostMode = racingCarBoostMode;
            await this.racingCarRepository.SaveChangesAsync();

            await Clients.GroupExcept(userWithLobby.Item2.LobbyName, Context.ConnectionId).SendAsync(RacingGameHubMethodNameConstants.CarBoosted, car);
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
                await Groups.RemoveFromGroupAsync(userWithLobby.Item2.LobbyName, Context.ConnectionId);
                await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.OtherCarWithIdHasBeenDeleted, car.Id);
            }
        }
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