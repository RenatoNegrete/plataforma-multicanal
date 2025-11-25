using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Models;
using PlataformaMulticanalFrontend.Services;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class CarritoController : Controller
    {

        private readonly OrdenService _ordenService;

        public CarritoController(OrdenService ordenService)
        {
            _ordenService = ordenService;
        }
        
        public IActionResult Index()
        {
            ViewData["Title"] = "Mi Carrito de Compras";
            
            return View();
        }

        // POST: Carrito/ProcederAlPago
        [HttpPost]
        public async Task<IActionResult> ProcederAlPago()
        {
            try
            {
                // Datos quemados para la demostración
                var ordenDto = new CrearOrdenDto
                {
                    ClienteId = 1,
                    ClienteMail = "renatonegrete563@gmail.com",
                    Productos = new List<ProductoOrdenDto>
                    {
                        new ProductoOrdenDto
                        {
                            ProductoId = "69235b519266cc0a159b1416",
                            Cantidad = 3
                        }
                    }
                };

                var response = await _ordenService.CrearOrdenTestAsync(ordenDto);

                if (response.Success)
                {
                    TempData["Success"] = response.Message;
                    return Json(new { 
                        success = true, 
                        message = "¡Orden creada exitosamente!",
                        data = response.Data 
                    });
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = response.Message,
                        error = response.ErrorMessage 
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception en ProcederAlPago: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = "Error al procesar la orden",
                    error = ex.Message 
                });
            }
        }

    }

}