using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;
using PlataformaMulticanalFrontend.Models;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly CatalogoService _catalogoService;
        private readonly ILogger<CatalogoController> _logger;

        public CatalogoController(CatalogoService catalogoService, ILogger<CatalogoController> logger)
        {
            _catalogoService = catalogoService;
            _logger = logger;
        }

        // GET: Catalogo/Index
        public async Task<IActionResult> Index(string? buscar, string? categoria, int pagina = 1)
        {
            ViewData["Title"] = "Catálogo de Productos";
            
            try
            {
                List<Producto> productos;

                // Filtrar por categoría si se especifica
                if (!string.IsNullOrEmpty(categoria) && categoria != "todas")
                {
                    productos = await _catalogoService.ObtenerPorCategoriaAsync(categoria);
                }
                else
                {
                    productos = await _catalogoService.ObtenerTodosAsync();
                }

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
                ViewBag.Categorias = await _catalogoService.ObtenerCategoriasAsync();

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

                // ViewBag para la vista
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalProductos = totalProductos;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.BuscarTexto = buscar;
                ViewBag.CategoriaSeleccionada = categoria;

                return View(productosPaginados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el catálogo");
                TempData["Error"] = "Error al cargar el catálogo de productos";
                return View(new List<Producto>());
            }
        }

        // GET: Catalogo/Detalle/id
        public async Task<IActionResult> Detalle(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var producto = await _catalogoService.ObtenerPorIdAsync(id);
                
                if (producto == null)
                {
                    TempData["Error"] = "El producto no existe";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["Title"] = producto.Nombre;

                // Obtener productos relacionados de la misma categoría
                if (!string.IsNullOrEmpty(producto.Categoria))
                {
                    var productosRelacionados = await _catalogoService.ObtenerPorCategoriaAsync(producto.Categoria);
                    ViewBag.ProductosRelacionados = productosRelacionados
                        .Where(p => p.Id != id)
                        .Take(4)
                        .ToList();
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el detalle del producto {Id}", id);
                TempData["Error"] = "Error al cargar el producto";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Catalogo/Categoria/nombre
        public async Task<IActionResult> Categoria(string nombre, int pagina = 1)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["Title"] = $"Categoría: {nombre}";

            try
            {
                var productos = await _catalogoService.ObtenerPorCategoriaAsync(nombre);

                // Paginación
                int productosPorPagina = 12;
                var totalProductos = productos.Count;
                var totalPaginas = (int)Math.Ceiling(totalProductos / (double)productosPorPagina);
                
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
                ViewBag.CategoriaNombre = nombre;

                return View(productosPaginados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar productos de la categoría {Categoria}", nombre);
                TempData["Error"] = "Error al cargar productos de la categoría";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Catalogo/Buscar
        public async Task<IActionResult> Buscar(string query, int pagina = 1)
        {
            ViewData["Title"] = $"Búsqueda: {query}";

            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var productos = await _catalogoService.BuscarAsync(query);

                // Paginación
                int productosPorPagina = 12;
                var totalProductos = productos.Count;
                var totalPaginas = (int)Math.Ceiling(totalProductos / (double)productosPorPagina);
                
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
                ViewBag.QueryBusqueda = query;

                return View(productosPaginados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos con query: {Query}", query);
                TempData["Error"] = "Error al realizar la búsqueda";
                return RedirectToAction(nameof(Index));
            }
        }

        // API endpoint para búsqueda AJAX (opcional)
        [HttpGet]
        public async Task<IActionResult> BuscarJson(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<Producto>());
            }

            try
            {
                var productos = await _catalogoService.BuscarAsync(query);
                return Json(productos.Take(10)); // Limitar a 10 resultados para autocomplete
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en búsqueda AJAX");
                return Json(new List<Producto>());
            }
        }

        // API endpoint para obtener categorías (opcional)
        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            try
            {
                var categorias = await _catalogoService.ObtenerCategoriasAsync();
                return Json(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return Json(new List<string>());
            }
        }
    }
}