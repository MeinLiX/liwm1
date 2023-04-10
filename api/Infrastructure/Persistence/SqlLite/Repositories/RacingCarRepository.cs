using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SqlLite.Repositories;

public class RacingCarRepository : IRacingCarRepository
{
    private readonly IDataContext dataContext;

    public RacingCarRepository(IDataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task DeleteRacingCarByIdAsync(int id)
    {
        var carToDelete = await GetRacingCarByIdAsync(id);
        if (carToDelete != null)
        {
            this.dataContext.RacingCars.Remove(carToDelete);
        }
    }

    public async Task<RacingCar?> GetRacingCarByIdAsync(int id) => await this.dataContext.RacingCars.FirstOrDefaultAsync(rc => rc.Id == id);
}