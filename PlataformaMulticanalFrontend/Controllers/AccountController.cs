using Microsoft.AspNetCore.Mvc;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string? returnUrl)
        {
            ViewData["Title"] = "Iniciar Sesión";
            ViewData["ReturnUrl"] = returnUrl;
            
            return View();
        }

        public IActionResult Registro()
        {
            ViewData["Title"] = "Crear Cuenta";
            
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string? returnUrl)
        {
            // Lógica de login aquí
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Registro(string nombre, string email, string password)
        {
            // Lógica de registro aquí
            return RedirectToAction("Index", "Home");
        }
    }
}