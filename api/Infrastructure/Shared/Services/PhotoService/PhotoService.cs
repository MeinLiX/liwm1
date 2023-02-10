using Application.Interfaces;

namespace Shared.Services.PhotoService;

public class PhotoService : IPhotoService
{
    private readonly ICloudinaryService cloudinaryService;

    public PhotoService(ICloudinaryService cloudinaryService)
    {
        this.cloudinaryService = cloudinaryService;
    }

    public Task<string> GetGamePhotoAsync(string gameName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>> GetUsersPhotosAsync() => await this.cloudinaryService.GetPhotosAsync();
}