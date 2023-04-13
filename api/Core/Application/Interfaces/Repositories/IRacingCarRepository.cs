using Domain.Entities;

namespace Application.Interfaces;

public interface IRacingCarRepository
{
    Task AddRacingCarAsync(RacingCar racingCar);
    Task<RacingCar?> GetRacingCarByIdAsync(int id);
    Task DeleteRacingCarByIdAsync(int id);
}