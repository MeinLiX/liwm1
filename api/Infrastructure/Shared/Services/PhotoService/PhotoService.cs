using Application.Interfaces;

namespace Shared.Services.PhotoService;

public class PhotoService : IPhotoService
{
    private readonly ICloudinaryService cloudinaryService;

    public PhotoService(ICloudinaryService cloudinaryService)
    {
        this.cloudinaryService = cloudinaryService;
    }

    public async Task<string> GetGamePhotoAsync(string gameName) => await this.cloudinaryService.GetGamePhotoAsync(gameName);

    public async Task<List<string>> GetUsersPhotosAsync() => await this.cloudinaryService.GetUsersPhotosAsync();
}