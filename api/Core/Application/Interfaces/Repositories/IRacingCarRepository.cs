using Domain.Entities;
using Domain.Models;

namespace Application.Interfaces;

public interface IRacingCarRepository
{
    Task AddRacingCarAsync(RacingCar racingCar);
    Task<RacingCar?> GetRacingCarByIdAsync(int id);
    Task<RacingCar?> GetRacingCarByRacerNameAsync(string racerName);
    Task DeleteRacingCarByIdAsync(int id);
    Task FinishAsync(RacingCar car);
    Task BoostAsync(RacingCar car, RacingCarBoostMode boostMode);
    Task ChangeReadyStateAsync(RacingCar car, bool isReady);
}