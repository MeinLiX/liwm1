using Domain.Entities;

namespace Application.Interfaces;

public interface IRacingCarRepository
{
    Task AddRacingCar(RacingCar racingCar);
    Task<RacingCar?> GetRacingCarByIdAsync(int id);
    Task DeleteRacingCarByIdAsync(int id);
}