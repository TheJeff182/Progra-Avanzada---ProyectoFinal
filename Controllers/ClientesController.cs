using ProyectoFinal.Data;
using ProyectoFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProyectoFinal.Controllers;

public class ClientesController : Controller
{
    private readonly PixelTicoContext _context;
    private readonly ILogger<ClientesController> _logger;
    private const int PageSize = 10;

    public ClientesController(PixelTicoContext context, ILogger<ClientesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Clientes
    public async Task<IActionResult> Index(string? buscar, int page = 1)
    {
        try
        {
            var query = _context.Clientes.AsQueryable();

            // Buscar por nombre o cedula
            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(c => c.Nombre.Contains(buscar) || c.Cedula.Contains(buscar));
            }

            // Contar total de registros
            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)PageSize);

            // Validar pagina
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var clientes = await query
                .OrderBy(c => c.Nombre)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewData["buscar"] = buscar;
            ViewData["currentPage"] = page;
            ViewData["totalPages"] = totalPages;

            return View(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes");
            ModelState.AddModelError("", "Error al cargar los clientes");
            return View(new List<Cliente>());
        }
    }

    // GET: Clientes/Details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    // GET: Clientes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Clientes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Cedula,Correo,Telefono,Direccion")] Cliente cliente)
    {
        // Validar que la cedula sea única
        if (await _context.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula))
        {
            ModelState.AddModelError("Cedula", "La cedula ya existe en el sistema");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cliente creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                ModelState.AddModelError("", "Error al guardar el cliente");
            }
        }

        return View(cliente);
    }

    // GET: Clientes/Edit
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    // POST: Clientes/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Cedula,Correo,Telefono,Direccion")] Cliente cliente)
    {
        if (id != cliente.Id)
            return NotFound();

        // Validar que cedula no exista ya en sistema
        if (await _context.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula && c.Id != id))
        {
            ModelState.AddModelError("Cedula", "La cedula ya existe en el sistema");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cliente actualizado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al actualizar cliente");
                if (!await ClienteExists(cliente.Id))
                    return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente");
                ModelState.AddModelError("", "Error al guardar los cambios");
            }
        }

        return View(cliente);
    }

    // GET: Clientes/Delete
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    // POST: Clientes/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cliente eliminado correctamente";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cliente");
            TempData["error"] = "Error al eliminar el cliente";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<bool> ClienteExists(int id)
    {
        return await _context.Clientes.AnyAsync(e => e.Id == id);
    }

    // GET: Clientes/Buscar?termino=xxx
    // Endpoint API (JSON) usado por AJAX en la pantalla de Ventas.
    [HttpGet]
    public async Task<IActionResult> Buscar(string? termino)
    {
        try
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                query = query.Where(c => c.Nombre.Contains(termino) || c.Cedula.Contains(termino));
            }

            var clientes = await query
                .OrderBy(c => c.Nombre)
                .Take(10)
                .Select(c => new
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    cedula = c.Cedula
                })
                .ToListAsync();

            return Json(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar clientes");
            return StatusCode(500, new { error = "Error al buscar clientes" });
        }
    }

    public class CrearClienteRapidoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
    }

    // POST: Clientes/CrearRapido
    // Endpoint API (JSON) usado por AJAX desde Ventas, para no obligar a salir de esa pantalla
    // cuando el cliente que se busca todavía no existe en el sistema.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearRapido([FromBody] CrearClienteRapidoDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Cedula))
        {
            return BadRequest(new { error = "Nombre y cédula son requeridos" });
        }

        var cliente = new Cliente
        {
            Nombre = request.Nombre.Trim(),
            Cedula = request.Cedula.Trim(),
            Correo = string.IsNullOrWhiteSpace(request.Correo) ? null : request.Correo.Trim(),
            Telefono = string.IsNullOrWhiteSpace(request.Telefono) ? null : request.Telefono.Trim()
        };

        if (!TryValidateModel(cliente))
        {
            var errores = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { error = string.Join(" ", errores) });
        }

        if (await _context.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula))
        {
            return BadRequest(new { error = "Ya existe un cliente con esa cédula" });
        }

        try
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = cliente.Id,
                nombre = cliente.Nombre,
                cedula = cliente.Cedula
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente rápido");
            return StatusCode(500, new { error = "Error al guardar el cliente" });
        }
    }
}