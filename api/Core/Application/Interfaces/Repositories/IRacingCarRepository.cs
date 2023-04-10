using Domain.Entities;

namespace Application.Interfaces;

public interface IRacingCarRepository
{
    Task<RacingCar?> GetRacingCarByIdAsync(int id);
    Task DeleteRacingCarByIdAsync(int id);
}