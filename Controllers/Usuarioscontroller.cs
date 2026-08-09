using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers;

// Roles válidos del sistema. Cualquier usuario debe tener exactamente uno de estos.
public static class Roles
{
    public const string Admin = "Admin";
    public const string Ventas = "Ventas";
    public const string Operaciones = "Operaciones";

    public static readonly string[] Todos = { Admin, Ventas, Operaciones };
}

public class UsuarioRolViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RolActual { get; set; } = "(sin rol)";
}

[Authorize(Roles = Roles.Admin)]
public class UsuariosController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(UserManager<IdentityUser> userManager, ILogger<UsuariosController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        var usuarios = _userManager.Users.OrderBy(u => u.Email).ToList();
        var modelo = new List<UsuarioRolViewModel>();

        foreach (var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            modelo.Add(new UsuarioRolViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email ?? usuario.UserName ?? "(sin correo)",
                RolActual = roles.FirstOrDefault() ?? "(sin rol)"
            });
        }

        return View(modelo);
    }

    // GET: Usuarios/CambiarRol/{id}
    public async Task<IActionResult> CambiarRol(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
            return NotFound();

        var rolesActuales = await _userManager.GetRolesAsync(usuario);

        ViewData["roles"] = Roles.Todos;
        var modelo = new UsuarioRolViewModel
        {
            Id = usuario.Id,
            Email = usuario.Email ?? usuario.UserName ?? "(sin correo)",
            RolActual = rolesActuales.FirstOrDefault() ?? "(sin rol)"
        };

        return View(modelo);
    }

    // POST: Usuarios/CambiarRol/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarRol(string id, string nuevoRol)
    {
        if (string.IsNullOrEmpty(id) || !Roles.Todos.Contains(nuevoRol))
        {
            TempData["ErrorMessage"] = "Rol inválido";
            return RedirectToAction(nameof(Index));
        }

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
            return NotFound();

        var rolesActuales = await _userManager.GetRolesAsync(usuario);

        // Protección: no permitir que un Admin se quite a sí mismo el rol Admin
        // si es el único Admin que queda en el sistema (evita quedarse sin acceso).
        if (rolesActuales.Contains(Roles.Admin) && nuevoRol != Roles.Admin)
        {
            var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            if (admins.Count <= 1)
            {
                TempData["ErrorMessage"] = "No se puede quitar el rol Admin: es el único administrador del sistema.";
                return RedirectToAction(nameof(Index));
            }
        }

        try
        {
            if (rolesActuales.Any())
            {
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            }

            await _userManager.AddToRoleAsync(usuario, nuevoRol);

            TempData["SuccessMessage"] = $"Rol de {usuario.Email} actualizado a {nuevoRol}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar el rol del usuario {UserId}", id);
            TempData["ErrorMessage"] = "Ocurrió un error al cambiar el rol";
            return RedirectToAction(nameof(Index));
        }
    }
}