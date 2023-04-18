using Domain.Entities;
using Domain.Models;

namespace Application.Interfaces;

public interface IRacingCarRepository
{
    Task AddRacingCarAsync(RacingCar racingCar);
    Task<RacingCar?> GetRacingCarByIdAsync(int id);
    Task<RacingCar?> GetRacingCarByRacerNameAsync(string racerName);
    Task DeleteRacingCarByIdAsync(int id);
    Task<int> SaveChangesAsync();
}