using Microsoft.AspNetCore.Mvc;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class CarritoController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Mi Carrito de Compras";
            
            return View();
        }
    }
}