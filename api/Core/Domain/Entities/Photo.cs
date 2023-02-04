using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; }
}