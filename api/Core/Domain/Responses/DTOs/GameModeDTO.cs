using Domain.Entities;

namespace Domain.Responses.DTOs;

public class GameModeDTO
{
    public string Name { get; set; }
    public string PreviewUrl { get; set; }

    public GameModeDTO(GameMode gameMode)
    {
        this.Name = gameMode.Name;
        this.PreviewUrl = gameMode.PreviewUrl;
    }
}