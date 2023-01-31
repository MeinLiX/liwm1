namespace Domain.Responses.DTOs;

public class UserDetailWithTokenDTO : UserDetailDTO
{
    public string Token { get; set; }
}