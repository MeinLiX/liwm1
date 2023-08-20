using Application.Interfaces;
using Domain.Entities;
using Domain.Responses;
using FluentValidation;
using MediatR;

namespace Application.Futures.GamesFuture;

public class GameModesGettingValidator : AbstractValidator<GameModesGettingRequest>
{
    public GameModesGettingValidator()
    {
    }
}

public class GameModesGettingRequest : IRequest<IRestResponseResult<List<GameMode>>>
{
    public int? id { get; set; }
    public string? name { get; set; }
    public int? start { get; set; }
    public int? count { get; set; }
}

public class GameModesGettingRequestHandler : IRequestHandler<GameModesGettingRequest, IRestResponseResult<List<GameMode>>>
{
    private readonly IGameModesRepository gameRepository;

    public GameModesGettingRequestHandler(IGameModesRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<IRestResponseResult<List<GameMode>>> Handle(GameModesGettingRequest request, CancellationToken cancellationToken)
    {
        var gameModes = new List<GameMode>();

        if (request.id.HasValue)
        {
            if (await this.gameRepository.GetGameModeByIdAsync(request.id.Value) is GameMode game)
            {
                gameModes.Add(game);
            }
        }
        else if (request.name != null)
        {
            if (await this.gameRepository.GetGameModeByNameAsync(request.name) is GameMode gameMode) 
            {
                gameModes.Add(gameMode);
            }
        }
        else if (request.start.HasValue && request.count.HasValue)
        {
            gameModes = await this.gameRepository.GetGameModesAsync(request.start.Value, request.count.Value);
        }
        else
        {
            gameModes = await this.gameRepository.GetGameModesAsync();
        }

        return RestResponseResult<List<GameMode>>.Success(gameModes);
    }
}