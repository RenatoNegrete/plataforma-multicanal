using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;
using PlataformaMulticanalFrontend.Models;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class AdminController : Controller
    {
        private readonly CatalogoService _catalogoService;

        public AdminController(CatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Dashboard Administrativo";
            
            // Obtener todos los productos
            var productos = await _catalogoService.ObtenerTodosAsync();
            
            // Calcular estadísticas de productos
            ViewBag.TotalProductos = productos.Count;
            ViewBag.ProductosStockBajo = productos.Count(p => p.Stock < 10);
            
            // Productos para mostrar (los primeros 3)
            ViewBag.TopProductos = productos.Take(3).ToList();
            
            // Categorías para el gráfico
            var categorias = productos
                .Where(p => !string.IsNullOrEmpty(p.Categoria))
                .GroupBy(p => p.Categoria)
                .Select(g => new { Categoria = g.Key, Cantidad = g.Count() })
                .ToList();
            
            ViewBag.CategoriasLabels = string.Join(",", categorias.Select(c => $"'{c.Categoria}'"));
            ViewBag.CategoriasData = string.Join(",", categorias.Select(c => c.Cantidad));
            
            return View();
        }

        // GET: Crear Producto
        public IActionResult CrearProducto()
        {
            return PartialView("_CrearProductoModal");
        }

        // POST: Crear Producto
        [HttpPost]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _catalogoService.CrearAsync(producto);
                
                if (resultado != null)
                {
                    TempData["Success"] = "Producto creado exitosamente";
                    return RedirectToAction(nameof(Dashboard));
                }
                
                TempData["Error"] = "Error al crear el producto";
            }
            
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Editar Producto
        public async Task<IActionResult> EditarProducto(string id)
        {
            var producto = await _catalogoService.ObtenerPorIdAsync(id);
            
            if (producto == null)
            {
                return NotFound();
            }
            
            return PartialView("_EditarProductoModal", producto);
        }

        // POST: Editar Producto
        [HttpPost]
        public async Task<IActionResult> EditarProducto(string id, Producto producto)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _catalogoService.ActualizarAsync(id, producto);
                
                if (resultado != null)
                {
                    TempData["Success"] = "Producto actualizado exitosamente";
                    return RedirectToAction(nameof(Dashboard));
                }
                
                TempData["Error"] = "Error al actualizar el producto";
            }
            
            return RedirectToAction(nameof(Dashboard));
        }

        // POST: Eliminar Producto
        [HttpPost]
        public async Task<IActionResult> EliminarProducto(string id)
        {
            var resultado = await _catalogoService.EliminarAsync(id);
            
            if (resultado)
            {
                TempData["Success"] = "Producto eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el producto";
            }
            
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Admin/Catalogo
public async Task<IActionResult> Catalogo(string? buscar, string? categoria, int pagina = 1)
{
    ViewData["Title"] = "Gestión de Catálogo";
    
    // Obtener todos los productos
    var productos = await _catalogoService.ObtenerTodosAsync();
    
    // Filtrar por búsqueda
    if (!string.IsNullOrEmpty(buscar))
    {
        productos = productos.Where(p => 
            p.Nombre.ToLower().Contains(buscar.ToLower()) ||
            (p.Descripcion != null && p.Descripcion.ToLower().Contains(buscar.ToLower()))
        ).ToList();
    }
    
    // Filtrar por categoría
    if (!string.IsNullOrEmpty(categoria))
    {
        productos = productos.Where(p => p.Categoria == categoria).ToList();
    }
    
    // Obtener categorías únicas para el filtro
    var categorias = await _catalogoService.ObtenerTodosAsync();
    ViewBag.Categorias = categorias
        .Where(p => !string.IsNullOrEmpty(p.Categoria))
        .Select(p => p.Categoria)
        .Distinct()
        .ToList();
    
    // Paginación simple
    int productosPorPagina = 10;
    var productosPaginados = productos
        .Skip((pagina - 1) * productosPorPagina)
        .Take(productosPorPagina)
        .ToList();
    
    ViewBag.PaginaActual = pagina;
    ViewBag.TotalProductos = productos.Count;
    ViewBag.TotalPaginas = (int)Math.Ceiling(productos.Count / (double)productosPorPagina);
    ViewBag.BuscarTexto = buscar;
    ViewBag.CategoriaSeleccionada = categoria;
    
    return View(productosPaginados);
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