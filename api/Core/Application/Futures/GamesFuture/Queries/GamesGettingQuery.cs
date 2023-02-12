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

public class GamesGettingRequest : IRequest<IRestResponseResult<List<Game>>>
{
    public int? id { get; set; }
    public int? start { get; set; }
    public int? count { get; set; }
}

public class GamesGettingRequestHandler : IRequestHandler<GamesGettingRequest, IRestResponseResult<List<Game>>>
{
    private readonly IGameRepository gameRepository;

    public GamesGettingRequestHandler(IGameRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<IRestResponseResult<List<Game>>> Handle(GamesGettingRequest request, CancellationToken cancellationToken)
    {
        var games = new List<Game>();

        if (request.id.HasValue)
        {
            if (await this.gameRepository.GetGameByIdAsync(request.id.Value) is Game game)
            {
                games.Add(game);
            }
        }
        else if (request.start.HasValue && request.count.HasValue)
        {
            games = await this.gameRepository.GetGamesAsync(request.start.Value, request.count.Value);
        }
        else
        {
            games = await this.gameRepository.GetGamesAsync();
        }

        return RestResponseResult<List<Game>>.Success(games);
    }
}