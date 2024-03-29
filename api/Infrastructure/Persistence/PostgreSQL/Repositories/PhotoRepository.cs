using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace PostgreSQL.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly IDataContext dataContext;

    public PhotoRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task<Photo?> GetPhotoByIdAsync(int id) => await this.dataContext.Photos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Photo>> GetPhotosAsync() => await this.dataContext.Photos.AsNoTracking().ToListAsync();

    public async Task<List<Photo>> GetPhotosAsync(int start, int count)
    {
        var photosCount = this.dataContext.Photos.Count();
        start = start.ToStart(photosCount);
        return await this.dataContext.Photos.Skip(start).Take(count.ToCount(start, photosCount)).AsNoTracking().ToListAsync();
    }
}