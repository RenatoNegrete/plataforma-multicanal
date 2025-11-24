using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly CatalogoService _catalogoService;

        public CatalogoController(CatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        // GET: /Catalogo
        public async Task<IActionResult> Index(string? categoria, string? orden)
        {
            ViewData["Title"] = "Catálogo de Productos";
            ViewData["CategoriaSeleccionada"] = categoria;
            ViewData["OrdenSeleccionado"] = orden;

            var productos = string.IsNullOrEmpty(categoria) 
                ? await _catalogoService.ObtenerTodosAsync()
                : await _catalogoService.FiltrarPorCategoriaAsync(categoria);

            // Ordenar productos
            if (!string.IsNullOrEmpty(orden))
            {
                productos = orden switch
                {
                    "precio-asc" => productos.OrderBy(p => p.Precio).ToList(),
                    "precio-desc" => productos.OrderByDescending(p => p.Precio).ToList(),
                    "nombre-asc" => productos.OrderBy(p => p.Nombre).ToList(),
                    "nombre-desc" => productos.OrderByDescending(p => p.Nombre).ToList(),
                    _ => productos
                };
            }

            return View(productos);
        }

        // GET: /Catalogo/Detalle/5
        public async Task<IActionResult> Detalle(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var producto = await _catalogoService.ObtenerPorIdAsync(id);
            
            if (producto == null)
            {
                return NotFound();
            }

            ViewData["Title"] = producto.Nombre;
            return View(producto);
        }

        // GET: /Catalogo/Buscar?q=laptop
        public async Task<IActionResult> Buscar(string q)
        {
            ViewData["Title"] = $"Resultados para: {q}";
            ViewData["Query"] = q;

            if (string.IsNullOrEmpty(q))
            {
                return RedirectToAction(nameof(Index));
            }

            var productos = await _catalogoService.BuscarAsync(q);
            return View("Index", productos);
        }

        // GET: /Catalogo/Ofertas
        public async Task<IActionResult> Ofertas()
        {
            ViewData["Title"] = "Ofertas del Día";
            
            var productos = await _catalogoService.ObtenerTodosAsync();
            // Aquí puedes agregar lógica para filtrar ofertas
            // Por ejemplo, productos con descuento o precio especial
            
            return View("Index", productos);
        }
    }
}