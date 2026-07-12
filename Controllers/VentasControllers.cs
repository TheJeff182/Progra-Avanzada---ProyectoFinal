using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers;

public class VentasController : Controller
{
    // GET: Ventas
    public IActionResult Index()
    {
        return View();
    }
}