using System.Diagnostics;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PixelTicoContext _context;

    public HomeController(ILogger<HomeController> logger, PixelTicoContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var errorViewModel = new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        };
        return View(errorViewModel);
    }
}
