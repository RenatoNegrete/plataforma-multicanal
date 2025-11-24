using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;
using System.Threading.Tasks;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioApiService _apiService;

        public AccountController()
        {
            _apiService = new UsuarioApiService();
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewData["Title"] = "Iniciar Sesión";
            ViewData["ReturnUrl"] = returnUrl;
            
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Por favor completa todos los campos";
                return View();
            }

            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            var response = await _apiService.LoginUsuario(loginDto);

            if (response.Success)
            {
                // Guardar información del usuario en la sesión
                HttpContext.Session.SetString("FirebaseToken", response.Data.IdToken);
                HttpContext.Session.SetString("UserId", response.Data.LocalId);
                HttpContext.Session.SetString("UserEmail", response.Data.Email);
                HttpContext.Session.SetString("RefreshToken", response.Data.RefreshToken);

                TempData["Success"] = "¡Bienvenido de nuevo!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = response.ErrorMessage ?? "Error al iniciar sesión";
            return View();
        }

        // GET: Account/Registro
        [HttpGet]
        public IActionResult Registro()
        {
            ViewData["Title"] = "Crear Cuenta";
            return View();
        }

        // POST: Account/Registro
        [HttpPost]
        public async Task<IActionResult> Registro(
            string firstName, 
            string lastName, 
            string email, 
            string password,
            string telefono,
            string direccion)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(firstName) || 
                string.IsNullOrWhiteSpace(lastName) || 
                string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Por favor completa todos los campos obligatorios";
                return View();
            }

            if (password.Length < 6)
            {
                TempData["Error"] = "La contraseña debe tener al menos 6 caracteres";
                return View();
            }

            var registroDto = new RegistroDto
            {
                Nombre = $"{firstName} {lastName}",
                Email = email,
                Password = password,
                Telefono = telefono ?? "",
                Direccion = direccion ?? ""
            };

            var response = await _apiService.RegistrarUsuario(registroDto);

            if (response.Success)
            {
                TempData["Success"] = "¡Registro exitoso! Ahora puedes iniciar sesión con tu cuenta.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = response.ErrorMessage ?? "Error al registrar usuario";
            return View();
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Has cerrado sesión correctamente";
            return RedirectToAction("Index", "Home");
        }
    }
}