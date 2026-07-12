using ProyectoFinal.Data;
using ProyectoFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProyectoFinal.Controllers;

public class PedidosController : Controller
{
    private readonly PixelTicoContext _context;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(PixelTicoContext context, ILogger<PedidosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // DTOs para el request de facturación (JSON enviado por AJAX desde Ventas).
    // Descuento es un PORCENTAJE (0-100), no un monto fijo.
    public class CrearPedidoItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; } = 0m;
    }

    public class CrearPedidoDto
    {
        public int ClienteId { get; set; }
        public List<CrearPedidoItemDto> Items { get; set; } = new();
    }

    // DTO para /api/pedidos/calcular (sección 3 del enunciado):
    // recibe líneas [{ productoId, cantidad, descuento }] y responde { subtotal, impuestos, total }.
    // Descuento también es PORCENTAJE (0-100) aquí.
    public class CalcularLineaDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; } = 0m;
    }

    private const int PageSize = 10;

    // Resultado del cálculo de una línea de pedido.
    private record LineaCalculada(decimal SubtotalLinea, decimal ImpuestoLinea, decimal TotalLinea, decimal MontoDescuento);

    // El descuento que llega del cliente es un PORCENTAJE (0-100) sobre el subtotal bruto de la línea.
    // El monto resultante en colones/dólares (MontoDescuento) es lo que se persiste en
    // PedidoDetalle.Descuento, ya que esa columna en la base de datos es un monto (decimal), no un %.
    private static LineaCalculada CalcularLinea(Producto producto, int cantidad, decimal descuentoPorc)
    {
        var subtotalBruto = producto.Precio * cantidad;

        // El porcentaje de descuento siempre debe quedar entre 0 y 100
        var porcAplicado = Math.Min(Math.Max(descuentoPorc, 0m), 100m);
        var montoDescuento = Math.Round(subtotalBruto * (porcAplicado / 100m), 2);

        var subtotalLinea = subtotalBruto - montoDescuento;
        var impuestoLinea = Math.Round(subtotalLinea * (producto.ImpuestoPorc / 100m), 2);
        var totalLinea = subtotalLinea + impuestoLinea;

        return new LineaCalculada(subtotalLinea, impuestoLinea, totalLinea, montoDescuento);
    }

    // GET: Pedidos
    // Historial de ventas realizadas.
    public async Task<IActionResult> Index(int page = 1)
    {
        try
        {
            var query = _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.Fecha)
                .AsQueryable();

            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)PageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages < 1 ? 1 : totalPages;

            var pedidos = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewData["currentPage"] = page;
            ViewData["totalPages"] = totalPages;

            return View(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el historial de ventas");
            ModelState.AddModelError("", "Error al cargar el historial de ventas");
            return View(new List<Pedido>());
        }
    }

    // GET: Pedidos/Details/5
    // Recibo/detalle de una venta ya facturada.
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Usuario)
            .Include(p => p.PedidoDetalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound();

        return View(pedido);
    }

    // POST: /api/pedidos/calcular
    // Endpoint conforme al enunciado del proyecto (sección 3): recibe las líneas del carrito
    // y recalcula subtotal/impuestos/total en el SERVIDOR (no confía en lo que muestra el navegador).
    // Es de solo lectura (no guarda nada), se usa para el "cálculo en vivo" mientras se arma el pedido.
    // También devuelve "descuento" (monto total rebajado en $) para mostrarlo en el carrito.
    // TODO: agregar [Authorize] cuando Identity esté integrado.
    [HttpPost]
    [Route("/api/pedidos/calcular")]
    public async Task<IActionResult> Calcular([FromBody] List<CalcularLineaDto>? lineas)
    {
        try
        {
            if (lineas == null || lineas.Count == 0)
            {
                return Json(new { subtotal = 0m, impuestos = 0m, total = 0m, descuento = 0m });
            }

            var productoIds = lineas.Select(l => l.ProductoId).Distinct().ToList();
            var productos = await _context.Productos
                .Where(p => productoIds.Contains(p.Id))
                .ToListAsync();

            decimal subtotal = 0m;
            decimal impuestos = 0m;
            decimal descuentoTotal = 0m;

            foreach (var linea in lineas)
            {
                var producto = productos.FirstOrDefault(p => p.Id == linea.ProductoId);
                if (producto == null || linea.Cantidad <= 0) continue;

                var calculo = CalcularLinea(producto, linea.Cantidad, linea.Descuento);
                subtotal += calculo.SubtotalLinea;
                impuestos += calculo.ImpuestoLinea;
                descuentoTotal += calculo.MontoDescuento;
            }

            return Json(new
            {
                subtotal = Math.Round(subtotal, 2),
                impuestos = Math.Round(impuestos, 2),
                total = Math.Round(subtotal + impuestos, 2),
                descuento = Math.Round(descuentoTotal, 2)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular totales del pedido");
            return StatusCode(500, new { error = "Error al calcular los totales" });
        }
    }

    // POST: Pedidos/Crear
    // Recibe el carrito armado en la pantalla de Ventas y crea el Pedido + PedidoDetalle.
    // Los precios e impuestos se recalculan en el servidor a partir de la base de datos
    // (nunca se confía en los valores que envía el navegador) y se valida el stock disponible.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear([FromBody] CrearPedidoDto request)
    {
        if (request == null || request.Items == null || request.Items.Count == 0)
        {
            return BadRequest(new { error = "El carrito está vacío" });
        }

        var cliente = await _context.Clientes.FindAsync(request.ClienteId);
        if (cliente == null)
        {
            return BadRequest(new { error = "Debe seleccionar un cliente válido" });
        }

        // TODO: reemplazar por el usuario autenticado cuando se implemente el login real.
        // Por ahora se usa el primer usuario registrado como "usuario del sistema".
        var usuario = await _context.Usuarios.FirstOrDefaultAsync();
        if (usuario == null)
        {
            return StatusCode(500, new { error = "No hay un usuario del sistema configurado" });
        }

        // Agrupar por si el mismo producto llegó repetido en el request.
        // El descuento es un porcentaje, así que en caso de duplicados no se suma (no tendría sentido
        // sumar dos porcentajes); simplemente se usa el que venga en la primera línea para ese producto.
        var itemsAgrupados = request.Items
            .Where(i => i.Cantidad > 0)
            .GroupBy(i => i.ProductoId)
            .Select(g => new CrearPedidoItemDto
            {
                ProductoId = g.Key,
                Cantidad = g.Sum(i => i.Cantidad),
                Descuento = g.First().Descuento
            })
            .ToList();

        if (itemsAgrupados.Count == 0)
        {
            return BadRequest(new { error = "El carrito está vacío" });
        }

        var productoIds = itemsAgrupados.Select(i => i.ProductoId).ToList();

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Se leen los productos DENTRO de la transacción y con bloqueo de fila (UPDLOCK, ROWLOCK).
            // Esto evita que dos ventas simultáneas del mismo producto pasen la validación de stock
            // con el mismo valor "viejo" y terminen vendiendo más unidades de las que hay: la segunda
            // petición espera a que la primera termine (commit o rollback) antes de leer el stock real.
            var productos = new List<Producto>();
            foreach (var id in productoIds)
            {
                var producto = await _context.Productos
                    .FromSqlInterpolated($"SELECT * FROM Producto WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}")
                    .FirstOrDefaultAsync();

                if (producto != null) productos.Add(producto);
            }

            // Validar que todos los productos existan, estén activos y tengan stock suficiente
            // (se revalida aquí, con los datos ya bloqueados, no con lo que se leyó antes de la transacción)
            foreach (var item in itemsAgrupados)
            {
                var producto = productos.FirstOrDefault(p => p.Id == item.ProductoId);

                if (producto == null || !producto.Activo)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { error = $"Producto {item.ProductoId} no disponible" });
                }

                if (producto.Stock < item.Cantidad)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { error = $"Stock insuficiente para '{producto.Nombre}' (disponible: {producto.Stock})" });
                }
            }

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                UsuarioId = usuario.Id,
                Fecha = DateTime.Now,
                Estado = "Completado"
            };

            decimal subtotal = 0m;
            decimal impuestos = 0m;

            foreach (var item in itemsAgrupados)
            {
                var producto = productos.First(p => p.Id == item.ProductoId);
                var calculo = CalcularLinea(producto, item.Cantidad, item.Descuento);

                pedido.PedidoDetalles.Add(new PedidoDetalle
                {
                    ProductoId = producto.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnit = producto.Precio,
                    Descuento = calculo.MontoDescuento, // se persiste el monto en $ ya calculado, no el %
                    ImpuestoPorc = producto.ImpuestoPorc,
                    TotalLinea = calculo.TotalLinea
                });

                producto.Stock -= item.Cantidad;

                subtotal += calculo.SubtotalLinea;
                impuestos += calculo.ImpuestoLinea;
            }

            pedido.Subtotal = subtotal;
            pedido.Impuestos = impuestos;
            pedido.Total = subtotal + impuestos;

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Json(new
            {
                success = true,
                pedidoId = pedido.Id,
                subtotal = pedido.Subtotal,
                impuestos = pedido.Impuestos,
                total = pedido.Total
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al crear el pedido");
            return StatusCode(500, new { error = "Ocurrió un error al procesar la venta" });
        }
    }
}