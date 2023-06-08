using Domain.Entities;
using Domain.Models;

namespace Domain.Responses.DTOs;

public class GameDTO
{
    public GameMode Mode { get; set; }
    public ICollection<UserDetailDTO> Players { get; set; }
    public ICollection<UserDetailDTO> RatingPlayers { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public GameState State { get; set; }

    public GameDTO(Game game)
    {
        this.Mode = game.Mode;
        // this.Players = game.Players.Select(p => new UserDetailDTO(p)).ToList();
        // this.RatingPlayers = game.RatingPlayers.Select(rp => new UserDetailDTO(rp)).ToList();
        this.CreatedAt = game.CreatedAt;
        this.State = game.State;
    }
}