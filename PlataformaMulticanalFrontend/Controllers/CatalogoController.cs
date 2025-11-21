using Microsoft.AspNetCore.Mvc;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class CatalogoController : Controller
    {
        public IActionResult Index(string? tipo, string? categoria, string? orden)
        {
            ViewData["Title"] = "Catálogo de Productos";
            ViewData["TipoSeleccionado"] = tipo;
            ViewData["CategoriaSeleccionada"] = categoria;
            ViewData["OrdenSeleccionado"] = orden;
            
            return View();
        }

        public IActionResult Detalle(int id)
        {
            ViewData["Title"] = "Detalle del Producto";
            ViewData["ProductoId"] = id;
            
            return View();
        }
    }
}