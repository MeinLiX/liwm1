using Domain.Entities;

namespace Domain.Responses.DTOs;

public class UserDetailDTO
{
    public int PhotoId { get; }
    public string Username { get; }
    public bool IsAnonymous { get; }
    public GameAppUsersStatsDTO Stats { get; }

    public UserDetailDTO()
    {

    }

    public UserDetailDTO(AppUser user)
    {
        this.PhotoId = user.PhotoId;
        this.Username = user.UserName;
        this.IsAnonymous = string.IsNullOrEmpty(user.PasswordHash);
    }

    public UserDetailDTO(GameAppUsersStats stats)
    {
        this.PhotoId = stats.AppUser.PhotoId;
        this.Username = stats.AppUser.UserName;
        this.IsAnonymous = string.IsNullOrEmpty(stats.AppUser.PasswordHash);
        this.Stats = new GameAppUsersStatsDTO
        {
            Place = stats.Place
        };
    }
}