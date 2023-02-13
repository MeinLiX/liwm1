using Domain.Entities;

namespace Domain.Responses.DTOs;

public class UserDetailDTO
{
    public int PhotoId { get; }
    public string Username { get; }
    public bool IsAnonymous { get; }

    public UserDetailDTO()
    {
        
    }

    public UserDetailDTO(AppUser user)
    {
        this.PhotoId = user.PhotoId;
        this.Username = user.UserName;
        this.IsAnonymous = false;
    }

    public UserDetailDTO(AnonymousUser user)
    {
        this.PhotoId = user.PhotoId;
        this.Username = user.UserName;
        this.IsAnonymous = true;
    }
}