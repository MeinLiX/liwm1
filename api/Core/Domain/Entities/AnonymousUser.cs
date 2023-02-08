using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("AnonymousUsers")]
public class AnonymousUser
{
    public int Id { get; set; }
    public int PhotoId { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string Role { get; private set; } = "Anonymous";
}