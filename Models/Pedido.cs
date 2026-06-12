using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models;

public class Pedido
{
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Range(0, 999999.99)]
    public decimal Subtotal { get; set; }

    [Range(0, 999999.99)]
    public decimal Impuestos { get; set; }

    [Range(0, 999999.99)]
    public decimal Total { get; set; }

    [StringLength(30)]
    public string Estado { get; set; } = "Pendiente";

    // Navegación
    public virtual Cliente? Cliente { get; set; }
    public virtual Usuario? Usuario { get; set; }
    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
