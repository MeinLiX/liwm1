using Application.Interfaces;
using Domain.Entities;
using Domain.Responses;
using FluentValidation;
using MediatR;

namespace Application.Futures.GamesFuture;

public class GamesGettingValidator : AbstractValidator<GamesGettingRequest>
{
    public GamesGettingValidator()
    {
    }
}

public class GamesGettingRequest : IRequest<IRestResponseResult<List<GameMode>>>
{
    public int? id { get; set; }
    public int? start { get; set; }
    public int? count { get; set; }
}

public class GamesGettingRequestHandler : IRequestHandler<GamesGettingRequest, IRestResponseResult<List<GameMode>>>
{
    private readonly IGameModesRepository gameRepository;

    public GamesGettingRequestHandler(IGameModesRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<IRestResponseResult<List<GameMode>>> Handle(GamesGettingRequest request, CancellationToken cancellationToken)
    {
        var games = new List<GameMode>();

        if (request.id.HasValue)
        {
            if (await this.gameRepository.GetGameModeByIdAsync(request.id.Value) is GameMode game)
            {
                games.Add(game);
            }
        }
        else if (request.start.HasValue && request.count.HasValue)
        {
            games = await this.gameRepository.GetGameModesAsync(request.start.Value, request.count.Value);
        }
        else
        {
            games = await this.gameRepository.GetGameModesAsync();
        }

        return RestResponseResult<List<GameMode>>.Success(games);
    }
}