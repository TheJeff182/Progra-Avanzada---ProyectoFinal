using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models;

public class PedidoDetalle
{
    public int Id { get; set; }

    [Required]
    public int PedidoId { get; set; }

    [Required]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }

    [Range(0, 999999.99)]
    public decimal PrecioUnit { get; set; }

    [Range(0, 999999.99)]
    public decimal Descuento { get; set; }

    [Range(0, 100)]
    public decimal ImpuestoPorc { get; set; }

    [Range(0, 999999.99)]
    public decimal TotalLinea { get; set; }

    // Navegación
    public virtual Pedido? Pedido { get; set; }
    public virtual Producto? Producto { get; set; }
}
