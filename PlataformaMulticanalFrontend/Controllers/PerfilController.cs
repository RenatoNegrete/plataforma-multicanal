using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;
using System.Threading.Tasks;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class PerfilController : Controller
    {
        private readonly PerfilApiService _perfilService;
        private readonly ILogger<PerfilController> _logger;

        public PerfilController(PerfilApiService perfilService, ILogger<PerfilController> logger)
        {
            _perfilService = perfilService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");

                _logger.LogInformation($"=== CARGANDO PERFIL === Email en sesión: {userEmail}");

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("⚠️ No hay email en sesión");
                    TempData["Error"] = "Debes iniciar sesión para ver tu perfil";
                    return RedirectToAction("Login", "Account");
                }

                var response = await _perfilService.ObtenerPorEmail(userEmail);

                if (!response.Success)
                {
                    _logger.LogError($"❌ Error: {response.ErrorMessage}");
                    TempData["Error"] = response.ErrorMessage;
                    ViewData["Title"] = "Mi Perfil";
                    return View();
                }

                if (response.Data == null)
                {
                    _logger.LogError("❌ Data es null");
                    TempData["Error"] = "No se encontró el usuario";
                    ViewData["Title"] = "Mi Perfil";
                    return View();
                }

                _logger.LogInformation($"✅ Perfil cargado - ID: {response.Data.Id}, Nombre: {response.Data.Nombre}");

                ViewData["Title"] = "Mi Perfil";
                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado");
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                ViewData["Title"] = "Mi Perfil";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil([FromBody] UsuarioUpdateDto request)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                _logger.LogInformation($"=== ACTUALIZANDO PERFIL === UserId: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Sesión expirada" });
                }

                var response = await _perfilService.ActualizarUsuario(long.Parse(userId), request);

                if (response.Success)
                {
                    if (!string.IsNullOrEmpty(request.Nombre))
                    {
                        HttpContext.Session.SetString("UserName", request.Nombre);
                    }

                    _logger.LogInformation("✅ Perfil actualizado");

                    return Json(new 
                    { 
                        success = true, 
                        message = "Perfil actualizado exitosamente",
                        data = response.Data 
                    });
                }

                _logger.LogWarning($"⚠️ Error: {response.ErrorMessage}");
                return Json(new { success = false, message = response.ErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        public IActionResult Historial()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "Debes iniciar sesión para ver tu historial";
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Mis Pedidos";
            return View();
        }
    }
}