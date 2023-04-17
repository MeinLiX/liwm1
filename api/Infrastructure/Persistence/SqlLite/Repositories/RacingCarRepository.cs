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

    public async Task AddRacingCarAsync(RacingCar racingCar)
    {
        await this.dataContext.RacingCars.AddAsync(racingCar);
        await this.dataContext.SaveChangesAsync();
    }

    public async Task DeleteRacingCarByIdAsync(int id)
    {
        var carToDelete = await GetRacingCarByIdAsync(id);
        if (carToDelete != null)
        {
            this.dataContext.RacingCars.Remove(carToDelete);
            await this.dataContext.SaveChangesAsync();
        }
    }

    public async Task<RacingCar?> GetRacingCarByIdAsync(int id) => await this.dataContext.RacingCars.FirstOrDefaultAsync(rc => rc.Id == id);

    public async Task<RacingCar?> GetRacingCarByRacerNameAsync(string racerName) => await this.dataContext.RacingCars.FirstOrDefaultAsync(rc => rc.RacerName == racerName);

    public async Task UpdateCarReadyStateAsync(RacingCar car, bool isReady)
    {
        car.IsReady = isReady;
        await this.dataContext.SaveChangesAsync();
    }
}