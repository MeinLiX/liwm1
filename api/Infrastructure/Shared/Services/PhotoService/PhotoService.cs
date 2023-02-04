using Application.Interfaces;
using Domain.Entities;

namespace Shared.Services.PhotoService;

public class PhotoService : IPhotoService
{
    private readonly ICloudinaryService cloudinaryService;

    public PhotoService(ICloudinaryService cloudinaryService)
    {
        this.cloudinaryService = cloudinaryService;
    }

    public async Task<List<string>> GetPhotosAsync() => await this.cloudinaryService.GetPhotosAsync();
}