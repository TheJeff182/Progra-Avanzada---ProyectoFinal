using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 1)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cedula es requerida")]
    [StringLength(20, MinimumLength = 1)]
    public string Cedula { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "El correo debe ser valido")]
    [StringLength(100)]
    public string? Correo { get; set; }

    [Phone(ErrorMessage = "El telefono no es valido")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(500)]
    public string? Direccion { get; set; }

    // Navegacion
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
