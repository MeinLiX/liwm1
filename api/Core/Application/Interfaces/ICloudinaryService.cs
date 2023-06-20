using Domain.Entities;

namespace Application.Interfaces;

public interface ICloudinaryService
{
    Task<List<string>> GetUsersPhotosAsync();
    Task<string?> GetGamePhotoAsync(string gameName);
}