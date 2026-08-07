using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoFinal.Controllers;

[Authorize(Roles = "Admin,Ventas")]
public class VentasController : Controller
{
    // GET: Ventas
    public IActionResult Index()
    {
        return View();
    }
}