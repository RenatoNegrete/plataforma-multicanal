using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;
using PlataformaMulticanalFrontend.Models;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly CatalogoService _catalogoService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(CatalogoService catalogoService, ILogger<HomeController> logger)
        {
            _catalogoService = catalogoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Obtener productos del backend
                var productos = await _catalogoService.ObtenerTodosAsync();
                
                // Tomar solo los primeros 12 productos para la página principal
                var productosDestacados = productos.Take(12).ToList();
                
                return View(productosDestacados);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar productos: {ex.Message}");
                
                // Si hay error, devolver lista vacía
                return View(new List<Producto>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}