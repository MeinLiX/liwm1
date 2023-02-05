using Domain.Entities;

namespace Application.Interfaces;

public interface IPhotoRepository
{
    Task<Photo?> GetPhotoByIdAsync(int id);
    Task<List<Photo>> GetPhotosAsync();
    Task<List<Photo>> GetPhotosAsync(int start, int count);
}