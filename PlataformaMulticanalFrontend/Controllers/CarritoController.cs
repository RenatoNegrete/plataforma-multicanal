using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Models;
using PlataformaMulticanalFrontend.Services;
using System.Text.Json;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class CarritoController : Controller
    {
        private readonly OrdenService _ordenService;
        private const string CARRITO_SESSION_KEY = "CarritoCompras";

        public CarritoController(OrdenService ordenService)
        {
            _ordenService = ordenService;
        }
        
        public IActionResult Index()
        {
            ViewData["Title"] = "Mi Carrito de Compras";
            var carrito = ObtenerCarrito();
            return View(carrito);
        }

        // POST: Carrito/Agregar
        [HttpPost]
        public IActionResult Agregar([FromBody] AgregarProductoDto dto)
        {
            try
            {
                var carrito = ObtenerCarrito();
                
                // Buscar si el producto ya existe en el carrito
                var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId);
                
                if (itemExistente != null)
                {
                    // Actualizar cantidad
                    itemExistente.Cantidad += dto.Cantidad;
                }
                else
                {
                    // Agregar nuevo item
                    carrito.Items.Add(new CarritoItemDto
                    {
                        ProductoId = dto.ProductoId,
                        Nombre = dto.Nombre,
                        Precio = dto.Precio,
                        Cantidad = dto.Cantidad,
                        ImagenUrl = dto.ImagenUrl
                    });
                }
                
                GuardarCarrito(carrito);
                
                return Json(new { 
                    success = true, 
                    message = "Producto agregado al carrito",
                    totalItems = carrito.Items.Sum(i => i.Cantidad)
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error al agregar producto",
                    error = ex.Message 
                });
            }
        }

        // POST: Carrito/ActualizarCantidad
        [HttpPost]
        public IActionResult ActualizarCantidad([FromBody] ActualizarCantidadDto dto)
        {
            try
            {
                var carrito = ObtenerCarrito();
                var item = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId);
                
                if (item != null)
                {
                    item.Cantidad = dto.Cantidad;
                    GuardarCarrito(carrito);
                    
                    return Json(new { 
                        success = true,
                        subtotal = carrito.CalcularSubtotal()
                    });
                }
                
                return Json(new { success = false, message = "Producto no encontrado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Carrito/Eliminar
        [HttpPost]
        public IActionResult Eliminar([FromBody] EliminarProductoDto dto)
        {
            try
            {
                var carrito = ObtenerCarrito();
                var item = carrito.Items.FirstOrDefault(i => i.ProductoId == dto.ProductoId);
                
                if (item != null)
                {
                    carrito.Items.Remove(item);
                    GuardarCarrito(carrito);
                    
                    return Json(new { 
                        success = true,
                        totalItems = carrito.Items.Sum(i => i.Cantidad)
                    });
                }
                
                return Json(new { success = false, message = "Producto no encontrado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Carrito/ObtenerTotal
        [HttpGet]
        public IActionResult ObtenerTotal()
        {
            var carrito = ObtenerCarrito();
            return Json(new { 
                totalItems = carrito.Items.Sum(i => i.Cantidad),
                subtotal = carrito.CalcularSubtotal()
            });
        }

        // POST: Carrito/ProcederAlPago
        [HttpPost]
        public async Task<IActionResult> ProcederAlPago()
        {
            try
            {
                var carrito = ObtenerCarrito();
                
                if (!carrito.Items.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = "El carrito está vacío" 
                    });
                }

                var ordenDto = new CrearOrdenDto
                {
                    ClienteId = 1, // TODO: Obtener del usuario logueado
                    ClienteMail = "renatonegrete563@gmail.com", // TODO: Obtener del usuario logueado
                    Productos = carrito.Items.Select(i => new ProductoOrdenDto
                    {
                        ProductoId = i.ProductoId,
                        Cantidad = i.Cantidad
                    }).ToList()
                };

                var response = await _ordenService.CrearOrdenTestAsync(ordenDto);

                if (response.Success)
                {
                    // Limpiar el carrito después de crear la orden
                    LimpiarCarrito();
                    
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

        #region Métodos Privados
        private CarritoDto ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString(CARRITO_SESSION_KEY);
            
            if (string.IsNullOrEmpty(carritoJson))
            {
                return new CarritoDto { Items = new List<CarritoItemDto>() };
            }
            
            return JsonSerializer.Deserialize<CarritoDto>(carritoJson) ?? new CarritoDto { Items = new List<CarritoItemDto>() };
        }

        private void GuardarCarrito(CarritoDto carrito)
        {
            var carritoJson = JsonSerializer.Serialize(carrito);
            HttpContext.Session.SetString(CARRITO_SESSION_KEY, carritoJson);
        }

        private void LimpiarCarrito()
        {
            HttpContext.Session.Remove(CARRITO_SESSION_KEY);
        }
        #endregion
    }
}