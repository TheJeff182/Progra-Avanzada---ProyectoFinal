using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models;

public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(150, MinimumLength = 1)]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal Precio { get; set; }

    [Range(0, 100, ErrorMessage = "El impuesto debe estar entre 0 y 100")]
    [DisplayFormat(DataFormatString = "{0:F2}")]
    public decimal ImpuestoPorc { get; set; } = 13.00m;

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    [StringLength(500)]
    public string? ImagenUrl { get; set; }

    public bool Activo { get; set; } = true;

    // Navegación
    public virtual Categoria? Categoria { get; set; }
    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
