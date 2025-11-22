using Microsoft.AspNetCore.Mvc;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class BusquedaController : Controller
    {
        public IActionResult Index(string? q, string? categoria, string? orden)
        {
            ViewData["Title"] = "Resultados de Búsqueda";
            ViewData["Query"] = q ?? "";
            ViewData["Categoria"] = categoria;
            ViewData["Orden"] = orden;
            
            return View();
        }
    }
}