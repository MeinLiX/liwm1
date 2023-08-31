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
public class RacingGameHub : Hub
{
    private readonly IUserRepository userRepository;
    private readonly ILobbyRepository lobbyRepository;
    private readonly IRacingCarRepository racingCarRepository;
    private readonly IGameRepository gameRepository;
    private readonly IGameModesRepository gameModesRepository;
    private readonly ILevelsRepository levelsRepository;

    public RacingGameHub(IUserRepository userRepository, ILobbyRepository lobbyRepository, IRacingCarRepository racingCarRepository, IGameRepository gameRepository, IGameModesRepository gameModesRepository, ILevelsRepository levelsRepository)
    {
        this.userRepository = userRepository;
        this.lobbyRepository = lobbyRepository;
        this.racingCarRepository = racingCarRepository;
        this.gameRepository = gameRepository;
        this.gameModesRepository = gameModesRepository;
        this.levelsRepository = levelsRepository;
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
                    State = GameState.Created,
                    Players = new List<AppUser>(userWithLobby.Item2.Users),
                    Stats = new List<GameAppUsersStats>(),
                    Mode = gameMode,
                    Lobby = userWithLobby.Item2,
                    LobbyId = userWithLobby.Item2.Id
                };

                foreach (var player in game.Players)
                {
                    game.Stats.Add(new GameAppUsersStats
                    {
                        AppUser = player,
                        Game = game
                    });
                }

                await this.lobbyRepository.CreateGameAsync(userWithLobby.Item2, game);
            }
            else
            {
                if (userWithLobby.Item2.CurrentGame.State != GameState.Created)
                {
                    await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.GameAlreadyStarted);
                    await Groups.AddToGroupAsync(Context.ConnectionId, userWithLobby.Item2.LobbyName);
                    return;
                }

                if (!userWithLobby.Item2.CurrentGame.Stats.Any(s => s.AppUser.Id == userWithLobby.Item1.Id))
                {
                    userWithLobby.Item2.CurrentGame.Players.Add(userWithLobby.Item1);
                    userWithLobby.Item2.CurrentGame.Stats.Add(new GameAppUsersStats
                    {
                        AppUser = userWithLobby.Item1,
                        Game = userWithLobby.Item2.CurrentGame
                    });
                    await this.gameRepository.AddUserToStatsAsync(userWithLobby.Item2.CurrentGame, userWithLobby.Item1);
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

            var cars = await this.GetRacingCarsExceptUserAsync(userWithLobby.Item1, userWithLobby.Item2);
            await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.CarCreated, cars, car);
            await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.RecievedNewRacingCar, car);
            await Groups.AddToGroupAsync(Context.ConnectionId, userWithLobby.Item2.LobbyName);
        }
    }

    public async Task UpdateCarReadyStateAsync(int carId, bool isReady)
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

            await this.racingCarRepository.ChangeReadyStateAsync(car, isReady);

            await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.CarReadyStateUpdated, car);

            var cars = await this.GetRacingCarsAsync(userWithLobby.Item1);
            if (cars is not null)
            {
                if (cars.All(c => c?.IsReady ?? false))
                {
                    await gameRepository.UpdateGameStateAsync(userWithLobby.Item2.CurrentGame, GameState.Started);
                    await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.GameStarting);
                }
            }
        }
    }

    private async Task<IEnumerable<RacingCar>?> GetRacingCarsAsync(AppUser user)
    {
        IEnumerable<RacingCar> cars = Enumerable.Empty<RacingCar>();

        var game = await this.gameRepository.GetGameWithPlayerAsync(user);
        if (game is not null)
        {
            cars = await Task.WhenAll(game.Players.Select(async p =>
                                                await this.racingCarRepository.GetRacingCarByRacerNameAsync(p.UserName))
                                                  .Where(c => c != null));
        }

        return cars;
    }

    private async Task<IEnumerable<RacingCar>?> GetRacingCarsExceptUserAsync(AppUser user, Lobby lobby)
    {
        IEnumerable<RacingCar> cars = Enumerable.Empty<RacingCar>();

        if (lobby.CurrentGame is not null)
        {
            cars = await Task.WhenAll(lobby.CurrentGame.Players.Where(p => p.UserName != user.UserName)
                                                               .Select(async p =>
                                                                await this.racingCarRepository.GetRacingCarByRacerNameAsync(p.UserName))
                                                               .Where(c => c != null));
        }

        return cars;
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

            await this.racingCarRepository.BoostAsync(car, racingCarBoostMode);

            await Clients.GroupExcept(userWithLobby.Item2.LobbyName, Context.ConnectionId).SendAsync(RacingGameHubMethodNameConstants.CarBoosted, car);
        }
    }

    public async Task FinishRacingAsync(int carId)
    {
        //UPDATE Games SET State = 0 WHERE Id = 10
        var userWithLobby = await GetUserWithLobbyAsync();
        if (userWithLobby.Item1 is not null && userWithLobby.Item2 is not null)
        {
            var car = await this.racingCarRepository.GetRacingCarByIdAsync(carId);
            if (car is null)
            {
                await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.NoSuchCar);
                return;
            }

            await this.racingCarRepository.FinishAsync(car);

            await this.lobbyRepository.RatePlayerAsync(userWithLobby.Item2, userWithLobby.Item1);

            await Clients.Caller.SendAsync(RacingGameHubMethodNameConstants.FinishedSuccessfully);
            await Clients.GroupExcept(userWithLobby.Item2.LobbyName, Context.ConnectionId).SendAsync(RacingGameHubMethodNameConstants.CarFinishedRacing, car);

            var cars = await this.GetRacingCarsAsync(userWithLobby.Item1);
            if (cars is not null)
            {
                var stats = userWithLobby.Item2.CurrentGame.Stats.FirstOrDefault(s => s.AppUser.UserName == car.RacerName);
                if (stats is not null)
                {
                    var points = stats.Place / stats.Game.Players.Count * 10;
                    await this.levelsRepository.AddPointsAsync(userWithLobby.Item1, userWithLobby.Item2.CurrentGame.Mode, points);
                }

                if (cars.All(c => c.IsFinished))
                {
                    await this.lobbyRepository.FinishGameAsync(userWithLobby.Item2);
                    await Clients.Group(userWithLobby.Item2.LobbyName).SendAsync(RacingGameHubMethodNameConstants.GameFinished, userWithLobby.Item2.PreviousGames.LastOrDefault().Stats.Select(rp => new UserDetailDTO(rp.AppUser)));

                    foreach (var carToDelete in cars)
                    {
                        await this.racingCarRepository.DeleteRacingCarByIdAsync(carToDelete.Id);
                    }
                }
            }
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