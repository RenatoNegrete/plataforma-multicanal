using Microsoft.AspNetCore.Mvc;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class PerfilController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Mi Perfil";
            
            return View();
        }

        public IActionResult Historial()
        {
            ViewData["Title"] = "Mis Pedidos";
            
            return View();
        }
    }
}