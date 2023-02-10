namespace Application.Interfaces;

public interface IPhotoService 
{
    Task<List<string>> GetUsersPhotosAsync();
    Task<string> GetGamePhotoAsync(string gameName);
}