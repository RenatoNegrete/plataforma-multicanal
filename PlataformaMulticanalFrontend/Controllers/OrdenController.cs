using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Models;
using PlataformaMulticanalFrontend.Services;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class OrdenController : Controller
    {
        private readonly OrdenService _ordenService;
        private readonly ILogger<OrdenController> _logger;

        public OrdenController(OrdenService ordenService, ILogger<OrdenController> logger)
        {
            _ordenService = ordenService;
            _logger = logger;
        }

        // GET: Orden
        public async Task<IActionResult> Index()
        {
            try
            {
                var ordenes = await _ordenService.ListarOrdenesAsync();
                return View(ordenes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar órdenes");
                TempData["Error"] = "Error al cargar las órdenes. Por favor, intente nuevamente.";
                return View(new List<Orden>());
            }
        }

        // GET: Orden/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID de orden no válido");
            }

            try
            {
                var orden = await _ordenService.ObtenerOrdenPorIdAsync(id);
                if (orden == null)
                {
                    TempData["Error"] = "Orden no encontrada";
                    return RedirectToAction(nameof(Index));
                }
                return View(orden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de la orden {OrdenId}", id);
                TempData["Error"] = "Error al cargar los detalles de la orden.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Orden/Create
        public IActionResult Create()
        {
            var orden = new Orden
            {
                Fecha = DateTime.Now,
                Productos = new List<string>()
            };
            return View(orden);
        }

        // POST: Orden/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Orden orden)
        {
            try
            {
                // Validaciones adicionales
                if (orden.ClienteId <= 0)
                {
                    ModelState.AddModelError("ClienteId", "El ID del cliente debe ser válido");
                }

                if (orden.Total <= 0)
                {
                    ModelState.AddModelError("Total", "El total debe ser mayor a cero");
                }

                if (orden.Productos == null || !orden.Productos.Any())
                {
                    ModelState.AddModelError("Productos", "Debe agregar al menos un producto");
                }

                if (ModelState.IsValid)
                {
                    var ordenCreada = await _ordenService.CrearOrdenAsync(orden);
                    TempData["Success"] = $"Orden #{ordenCreada?.Id} creada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                
                return View(orden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear orden");
                TempData["Error"] = "Error al crear la orden. Por favor, intente nuevamente.";
                return View(orden);
            }
        }

        // GET: Orden/MisOrdenes/5
        public async Task<IActionResult> MisOrdenes(long? clienteId)
        {
            if (!clienteId.HasValue || clienteId <= 0)
            {
                TempData["Error"] = "ID de cliente no válido";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var ordenes = await _ordenService.ListarOrdenesPorClienteAsync(clienteId.Value);
                ViewBag.ClienteId = clienteId.Value;
                return View(ordenes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar órdenes del cliente {ClienteId}", clienteId);
                TempData["Error"] = "Error al cargar las órdenes del cliente.";
                return View(new List<Orden>());
            }
        }

        // API endpoints para consumo AJAX
        [HttpPost]
        public async Task<IActionResult> CrearOrdenJson([FromBody] Orden orden)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { 
                        success = false, 
                        message = "Datos de orden inválidos",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var resultado = await _ordenService.CrearOrdenAsync(orden);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear orden vía JSON");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerOrdenJson(string id)
        {
            try
            {
                var orden = await _ordenService.ObtenerOrdenPorIdAsync(id);
                if (orden == null)
                {
                    return Json(new { success = false, message = "Orden no encontrada" });
                }
                return Json(new { success = true, data = orden });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener orden {OrdenId} vía JSON", id);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarOrdenesJson()
        {
            try
            {
                var ordenes = await _ordenService.ListarOrdenesAsync();
                return Json(new { success = true, data = ordenes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar órdenes vía JSON");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarOrdenesPorClienteJson(long clienteId)
        {
            try
            {
                var ordenes = await _ordenService.ListarOrdenesPorClienteAsync(clienteId);
                return Json(new { success = true, data = ordenes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar órdenes del cliente {ClienteId} vía JSON", clienteId);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}