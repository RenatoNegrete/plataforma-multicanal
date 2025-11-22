using Microsoft.AspNetCore.Mvc;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard Administrativo";
            
            return View();
        }

        public IActionResult Catalogo()
        {
            ViewData["Title"] = "Gestión de Catálogo";
            
            return View();
        }

        public IActionResult Proveedores()
        {
            ViewData["Title"] = "Gestión de Proveedores";
            
            return View();
        }

        public IActionResult Inventarios()
        {
            ViewData["Title"] = "Gestión de Inventarios";
            
            return View();
        }

        public IActionResult Precios()
        {
            ViewData["Title"] = "Gestión de Precios";
            
            return View();
        }

        public IActionResult Reportes()
        {
            ViewData["Title"] = "Reportes y Analytics";
            
            return View();
        }
    }
}