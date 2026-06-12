namespace ProyectoFinal.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // Navegacion
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
