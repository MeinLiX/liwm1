using Application.Interfaces;
using Domain.Entities;
using Domain.Responses;
using Domain.Responses.DTOs;
using FluentValidation;
using MediatR;

namespace Application.Futures.GameModesFuture;

public class GameModesGettingValidator : AbstractValidator<GameModesGettingRequest>
{
    public GameModesGettingValidator()
    {
    }
}

public class GameModesGettingRequest : IRequest<IRestResponseResult<IEnumerable<GameModeDTO>>>
{
    public int? id { get; set; }
    public string? name { get; set; }
    public int? start { get; set; }
    public int? count { get; set; }
}

public class GameModesGettingRequestHandler : IRequestHandler<GameModesGettingRequest, IRestResponseResult<IEnumerable<GameModeDTO>>>
{
    private readonly IGameModesRepository gameRepository;

    public GameModesGettingRequestHandler(IGameModesRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<IRestResponseResult<IEnumerable<GameModeDTO>>> Handle(GameModesGettingRequest request, CancellationToken cancellationToken)
    {
        var gameModes = new List<GameModeDTO>();

        if (request.id.HasValue)
        {
            if (await this.gameRepository.GetGameModeByIdAsync(request.id.Value) is GameMode game)
            {
                gameModes.Add(new GameModeDTO(game));
            }
        }
        else if (request.name != null)
        {
            if (await this.gameRepository.GetGameModeByNameAsync(request.name) is GameMode gameMode) 
            {
                gameModes.Add(new GameModeDTO(gameMode));
            }
        }
        else if (request.start.HasValue && request.count.HasValue)
        {
            gameModes = (await this.gameRepository.GetGameModesAsync(request.start.Value, request.count.Value)).Select(gm => new GameModeDTO(gm)).ToList();
        }
        else
        {
            gameModes = (await this.gameRepository.GetGameModesAsync()).Select(gm => new GameModeDTO(gm)).ToList();
        }

        return RestResponseResult<IEnumerable<GameModeDTO>>.Success(gameModes);
    }
}