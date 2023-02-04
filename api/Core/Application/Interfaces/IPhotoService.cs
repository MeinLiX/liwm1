using Domain.Entities;

namespace Application.Interfaces;

public interface IPhotoService 
{
    Task<List<string>> GetPhotosAsync();
}