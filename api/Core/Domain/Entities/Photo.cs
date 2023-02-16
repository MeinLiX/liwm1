using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Photos")]
public class Photo
{
    [Key]
    public int Id { get; set; }
    public string Url { get; set; }
}