using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;
using PlataformaMulticanalFrontend.Models;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly CatalogoService _catalogoService;
        private readonly ProveedorService _proveedorService;

        public ProveedorController(CatalogoService catalogoService, ProveedorService proveedorService)
        {
            _catalogoService = catalogoService;
            _proveedorService = proveedorService;
        }

        private bool VerificarSesionAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            
            return !string.IsNullOrEmpty(userRole) && 
                   !string.IsNullOrEmpty(userId) && 
                   userRole == "proveedor";
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Verificar autenticación
            if (!VerificarSesionAdmin())
            {
                TempData["Error"] = "Debes iniciar sesión como proveedor";
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Dashboard de Proveedor";
            
            try
            {
                // Obtener el ID del proveedor logueado
                var proveedorEmail = HttpContext.Session.GetString("UserEmail");
                var proveedores = await _proveedorService.ObtenerTodosAsync();
                var proveedor = proveedores.FirstOrDefault(p => p.Email == proveedorEmail);
                var proveedorId = proveedor?.Id;
                
                if (proveedorId == null)
                {
                    TempData["Error"] = "No se pudo identificar al proveedor. Por favor, inicia sesión nuevamente.";
                    return RedirectToAction("Login", "Account");
                }
                HttpContext.Session.SetString("ProveedorId", proveedorId.ToString());
                // Obtener todos los productos
                var todosLosProductos = await _catalogoService.ObtenerTodosAsync();

                // FILTRAR solo los productos del proveedor logueado
                var productos = todosLosProductos
                    .Where(p => p.ProveedorId == proveedorId.ToString())
                    .ToList();

                Console.WriteLine($"[DEBUG] Proveedor ID: {proveedorId}");
                Console.WriteLine($"[DEBUG] Productos del proveedor: {productos.Count}");
                
                // Calcular estadísticas de productos
                ViewBag.TotalProductos = productos.Count;
                ViewBag.ProductosSinStock = productos.Count(p => p.Stock == 0);
                ViewBag.ValorTotalInventario = productos.Sum(p => p.Precio * p.Stock);
                
                // CORREGIDO: Contar productos con stock bajo
                var countStockBajo = productos.Count(p => p.Stock > 0 && p.Stock < 10);
                ViewBag.ProductosStockBajo = countStockBajo;
                
                // Productos para mostrar (los 5 con mayor stock)
                ViewBag.TopProductos = productos
                    .OrderByDescending(p => p.Stock)
                    .Take(5)
                    .ToList();
                
                // Categorías para el gráfico
                var categorias = productos
                    .Where(p => !string.IsNullOrEmpty(p.Categoria))
                    .GroupBy(p => p.Categoria)
                    .Select(g => new { 
                        Categoria = g.Key, 
                        Cantidad = g.Count(),
                        ValorTotal = g.Sum(p => p.Precio * p.Stock)
                    })
                    .OrderByDescending(c => c.Cantidad)
                    .ToList();
                
                // CORREGIDO: Usar JsonSerializer para generar JSON válido
                if (categorias.Any())
                {
                    ViewBag.CategoriasLabels = System.Text.Json.JsonSerializer.Serialize(categorias.Select(c => c.Categoria).ToList());
                    ViewBag.CategoriasData = System.Text.Json.JsonSerializer.Serialize(categorias.Select(c => c.Cantidad).ToList());
                    ViewBag.CategoriasValor = System.Text.Json.JsonSerializer.Serialize(categorias.Select(c => c.ValorTotal).ToList());
                    
                    // Debug: Log para verificar
                    Console.WriteLine($"Labels: {ViewBag.CategoriasLabels}");
                    Console.WriteLine($"Data: {ViewBag.CategoriasData}");
                }
                else
                {
                    ViewBag.CategoriasLabels = null;
                    ViewBag.CategoriasData = null;
                    ViewBag.CategoriasValor = null;
                }
                
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el dashboard: {ex.Message}";
                
                // Valores por defecto en caso de error
                ViewBag.TotalProductos = 0;
                ViewBag.ProductosStockBajo = 0;
                ViewBag.ProductosSinStock = 0;
                ViewBag.ValorTotalInventario = 0;
                ViewBag.TopProductos = new List<Producto>();
                ViewBag.CategoriasLabels = null;
                ViewBag.CategoriasData = null;
                
                return View();
            }
        }

        // GET: Admin/Catalogo
        public async Task<IActionResult> Catalogo(string? buscar, string? categoria, int pagina = 1)
        {
            // Verificar autenticación
            if (!VerificarSesionAdmin())
            {
                TempData["Error"] = "Debes iniciar sesión como proveedor";
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Gestión de Catálogo";
            
            try
            {
                List<Producto> todosLosProductos;
                var proveedorId = HttpContext.Session.GetString("ProveedorId");

                // Filtrar por categoría si se especifica
                if (!string.IsNullOrEmpty(categoria) && categoria != "todas")
                {
                    todosLosProductos = await _catalogoService.ObtenerPorCategoriaAsync(categoria);
                }
                else
                {
                    todosLosProductos = await _catalogoService.ObtenerTodosAsync();
                }

                var productos = todosLosProductos
                    .Where(p => p.ProveedorId == proveedorId.ToString())
                    .ToList();

                // Filtrar por búsqueda
                if (!string.IsNullOrEmpty(buscar))
                {
                    productos = await _catalogoService.BuscarAsync(buscar);
                    
                    // Si también hay categoría, aplicar ambos filtros
                    if (!string.IsNullOrEmpty(categoria) && categoria != "todas")
                    {
                        productos = productos.Where(p => 
                            p.Categoria?.ToLower() == categoria.ToLower()
                        ).ToList();
                    }
                }
                
                // Obtener categorías únicas para el filtro
                var todasCategorias = await _catalogoService.ObtenerTodosAsync();
                ViewBag.Categorias = todasCategorias
                    .Where(p => !string.IsNullOrEmpty(p.Categoria))
                    .Select(p => p.Categoria)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
                
                // Paginación
                int productosPorPagina = 12;
                var totalProductos = productos.Count;
                var totalPaginas = (int)Math.Ceiling(totalProductos / (double)productosPorPagina);
                
                // Validar número de página
                if (pagina < 1) pagina = 1;
                if (pagina > totalPaginas && totalPaginas > 0) pagina = totalPaginas;
                
                var productosPaginados = productos
                    .OrderBy(p => p.Nombre)
                    .Skip((pagina - 1) * productosPorPagina)
                    .Take(productosPorPagina)
                    .ToList();
                
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalProductos = totalProductos;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.BuscarTexto = buscar;
                ViewBag.CategoriaSeleccionada = categoria;
                
                return View(productosPaginados);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el catálogo: {ex.Message}";
                return View(new List<Producto>());
            }
        }

        // GET: Admin/CrearProducto
        [HttpGet]
        public IActionResult CrearProducto()
        {
            if (!VerificarSesionAdmin())
            {
                return Unauthorized();
            }

            return PartialView("_CrearProductoModal", new Producto());
        }

        // POST: Admin/CrearProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            if (!VerificarSesionAdmin())
            {
                TempData["Error"] = "No tienes permisos para realizar esta acción";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _catalogoService.CrearAsync(producto);
                    
                    if (resultado != null)
                    {
                        TempData["Success"] = $"Producto '{producto.Nombre}' creado exitosamente";
                        return RedirectToAction(nameof(Catalogo));
                    }
                    
                    TempData["Error"] = "Error al crear el producto. Verifica los datos e intenta nuevamente.";
                }
                else
                {
                    TempData["Error"] = "Por favor completa todos los campos requeridos correctamente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al crear el producto: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Catalogo));
        }

        // GET: Admin/EditarProducto/id
        [HttpGet]
        public async Task<IActionResult> EditarProducto(string id)
        {
            if (!VerificarSesionAdmin())
            {
                return Unauthorized();
            }

            try
            {
                var producto = await _catalogoService.ObtenerPorIdAsync(id);
                
                if (producto == null)
                {
                    return NotFound();
                }
                
                return PartialView("_EditarProductoModal", producto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el producto: {ex.Message}";
                return RedirectToAction(nameof(Catalogo));
            }
        }

        // POST: Admin/EditarProducto/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProducto(string id, Producto producto)
        {
            if (!VerificarSesionAdmin())
            {
                TempData["Error"] = "No tienes permisos para realizar esta acción";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _catalogoService.ActualizarAsync(id, producto);
                    
                    if (resultado != null)
                    {
                        TempData["Success"] = $"Producto '{producto.Nombre}' actualizado exitosamente";
                        return RedirectToAction(nameof(Catalogo));
                    }
                    
                    TempData["Error"] = "No se pudo actualizar el producto. Verifica que exista.";
                }
                else
                {
                    TempData["Error"] = "Por favor completa todos los campos requeridos correctamente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar el producto: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Catalogo));
        }

        // POST: Admin/EliminarProducto/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(string id)
        {
            if (!VerificarSesionAdmin())
            {
                TempData["Error"] = "No tienes permisos para realizar esta acción";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var producto = await _catalogoService.ObtenerPorIdAsync(id);
                
                if (producto == null)
                {
                    TempData["Error"] = "El producto no existe";
                    return RedirectToAction(nameof(Catalogo));
                }

                var resultado = await _catalogoService.EliminarAsync(id);
                
                if (resultado)
                {
                    TempData["Success"] = $"Producto '{producto.Nombre}' eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "Error al eliminar el producto";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el producto: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Catalogo));
        }

        // GET: Admin/Inventarios
        public async Task<IActionResult> Inventarios()
        {
            if (!VerificarSesionAdmin())
            {
                TempData["Error"] = "Debes iniciar sesión como administrador";
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Gestión de Inventarios";
            
            try
            {
                var proveedorId = HttpContext.Session.GetString("ProveedorId");
                var todosLosProductos = await _catalogoService.ObtenerTodosAsync();

                var productos = todosLosProductos
                    .Where(p => p.ProveedorId == proveedorId.ToString())
                    .ToList();
                
                // Estadísticas de inventario
                ViewBag.ProductosSinStock = productos.Where(p => p.Stock == 0).ToList();
                ViewBag.ProductosStockBajo = productos.Where(p => p.Stock > 0 && p.Stock < 10).ToList();
                ViewBag.ProductosStockNormal = productos.Where(p => p.Stock >= 10).ToList();
                
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar inventarios: {ex.Message}";
                return View(new List<Producto>());
            }
        }

        // GET: Admin/Precios
        public async Task<IActionResult> Precios()
        {
            if (!VerificarSesionAdmin())
            {
                TempData["Error"] = "Debes iniciar sesión como administrador";
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Gestión de Precios";
            
            try
            {
                var proveedorId = HttpContext.Session.GetString("ProveedorId");
                var todosLosProductos = await _catalogoService.ObtenerTodosAsync();

                var productos = todosLosProductos
                    .Where(p => p.ProveedorId == proveedorId.ToString())
                    .ToList();
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar precios: {ex.Message}";
                return View(new List<Producto>());
            }
        }

    }
}
