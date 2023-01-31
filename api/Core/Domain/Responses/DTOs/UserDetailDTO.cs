using Domain.Entities;

namespace Domain.Responses.DTOs;

public class UserDetailDTO
{
    public int PhotoId { get; set; }
    public string Username { get; set; }

    public UserDetailDTO()
    {
        
    }

    public UserDetailDTO(AppUser user)
    {
        this.PhotoId = user.PhotoId;
        this.Username = user.UserName;
    }
}