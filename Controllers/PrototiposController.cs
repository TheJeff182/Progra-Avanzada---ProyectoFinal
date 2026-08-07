using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoFinal.Data;
using Microsoft.EntityFrameworkCore;

namespace ProyectoFinal.Controllers;

[Authorize(Roles = "Admin")]
public class PrototiposController : Controller
{
    private readonly PixelTicoContext _context;

    public PrototiposController(PixelTicoContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    public async Task<IActionResult> Reportes()
    {
        // 1. Ventas totales de todos los pedidos
        decimal ventasTotales = await _context.Pedidos
            .SumAsync(p => p.Total);

        // 2. Productos vendidos (cantidad de unidades de todos los tiempos)
        int productosVendidos = await _context.PedidoDetalles
            .SumAsync(pd => pd.Cantidad);

        // 3. Productos con stock bajo (menor a 10 unidades)
        var productosStockBajo = await _context.Productos
            .Where(p => p.Stock < 10 && p.Activo)
            .OrderBy(p => p.Stock)
            .Select(p => new ProductoStockBajoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Stock = p.Stock,
                CategoriaNombre = p.Categoria!.Nombre,
                Precio = p.Precio
            })
            .ToListAsync();

        var reporteData = new ReporteDto
        {
            VentasTotales = ventasTotales,
            ProductosVendidos = productosVendidos,
            ProductosStockBajo = productosStockBajo,
            FechaInicio = new DateTime(2000, 1, 1), // Fecha genérica para la vista
            FechaFin = DateTime.Today
        };

        return View(reporteData);
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Usuarios()
    {
        return View();
    }
}

// DTOs para los reportes
public class ProductoStockBajoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}

public class ReporteDto
{
    public decimal VentasTotales { get; set; }
    public int ProductosVendidos { get; set; }
    public List<ProductoStockBajoDto> ProductosStockBajo { get; set; } = new();
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}