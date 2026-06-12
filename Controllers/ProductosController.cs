using ProyectoFinal.Data;
using ProyectoFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ProyectoFinal.Controllers;

public class ProductosController : Controller
{
    private readonly PixelTicoContext _context;
    private readonly ILogger<ProductosController> _logger;
    private const int PageSize = 10;

    public ProductosController(PixelTicoContext context, ILogger<ProductosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Productos
    public async Task<IActionResult> Index(string? nombre, int? categoriaId, int page = 1)
    {
        // Cargar categorias 
        ViewData["categories"] = await _context.Categorias.ToListAsync();

        try
        {
            var query = _context.Productos
                .Include(p => p.Categoria)
                .AsQueryable();

            // Filtrar por nombre
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(p => p.Nombre.Contains(nombre));
            }

            // Filtrar por categoria
            if (categoriaId.HasValue && categoriaId > 0)
            {
                query = query.Where(p => p.CategoriaId == categoriaId);
            }

            // Contar total de registros
            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)PageSize);

            // Validar página
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var productos = await query
                .OrderBy(p => p.Nombre)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewData["nombre"] = nombre;
            ViewData["categoriaId"] = categoriaId;
            ViewData["currentPage"] = page;
            ViewData["totalPages"] = totalPages;

            return View(productos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            ModelState.AddModelError("", "Error al cargar los productos");
            return View(new List<Producto>());
        }
    }

    // GET: Productos/Details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
            return NotFound();

        return View(producto);
    }

    // GET: Productos/Create
    public async Task<IActionResult> Create()
    {
        ViewData["categories"] = await _context.Categorias.ToListAsync();
        return View();
    }

    // POST: Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,CategoriaId,Precio,ImpuestoPorc,Stock,ImagenUrl,Activo")] Producto producto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                TempData["success"] = "Producto creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                ModelState.AddModelError("", "Error al guardar el producto");
            }
        }

        ViewData["categories"] = await _context.Categorias.ToListAsync();
        return View(producto);
    }

    // GET: Productos/Edit
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
            return NotFound();

        ViewData["categories"] = await _context.Categorias.ToListAsync();
        return View(producto);
    }

    // POST: Productos/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,CategoriaId,Precio,ImpuestoPorc,Stock,ImagenUrl,Activo")] Producto producto)
    {
        if (id != producto.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(producto);
                await _context.SaveChangesAsync();
                TempData["success"] = "Producto actualizado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al actualizar producto");
                if (!await ProductoExists(producto.Id))
                    return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto");
                ModelState.AddModelError("", "Error al guardar los cambios");
            }
        }

        ViewData["categories"] = await _context.Categorias.ToListAsync();
        return View(producto);
    }

    // GET: Productos/Delete/
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
            return NotFound();

        return View(producto);
    }

    // POST: Productos/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                TempData["success"] = "Producto eliminado correctamente";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto");
            TempData["error"] = "Error al eliminar el producto";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<bool> ProductoExists(int id)
    {
        return await _context.Productos.AnyAsync(e => e.Id == id);
    }
}
