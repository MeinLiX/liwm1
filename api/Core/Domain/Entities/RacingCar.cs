using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;

namespace Domain.Entities;

[Table("RacingCars")]
public class RacingCar
{
    public int Id { get; set; }
    public string RacerName { get; set; }
    public RacingCarBoostMode RacingCarBoostMode { get; set; }
    public bool IsReady { get; set; } = false;
}