using Domain.Entities;
using Domain.Models;

namespace Domain.Responses.DTOs;

public class GameDTO
{
    public GameModeDTO Mode { get; set; }
    public ICollection<UserDetailDTO> Players { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public GameState State { get; set; }

    public GameDTO(Game game)
    {
        if (game.Mode != null)
        {
            this.Mode = new GameModeDTO(game.Mode);
        }

        this.Players = game.Players.Select(p => new UserDetailDTO(p)).ToList();
        this.CreatedAt = game.CreatedAt;
        this.State = game.State;
    }
}