using Domain.Entities;

namespace Domain.Responses.DTOs;

public class UserDetailWithTokenDTO : UserDetailDTO
{
    public string Token { get; set; }

    public UserDetailWithTokenDTO()
    {
        
    }

    public UserDetailWithTokenDTO(AppUser user) : base(user)
    {
        
    }
}