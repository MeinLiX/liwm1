using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;

namespace Domain.Entities;

[Table("Connections")]
public class Connection
{
    public string ConnectionId { get; set; }
    public string Username { get; set; }
    public ConnectionState ConnectionState { get; set; }
    public Lobby Lobby { get; set; }
}