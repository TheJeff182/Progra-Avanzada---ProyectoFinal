using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    public string? PasswordHash { get; set; }

    [Required]
    [StringLength(30)]
    public string Rol { get; set; } = "Ventas";

    // Navegación
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
