using Domain.Entities;

namespace Application.Interfaces;

public interface ICloudinaryService
{
    Task<List<string>> GetPhotosAsync();
}