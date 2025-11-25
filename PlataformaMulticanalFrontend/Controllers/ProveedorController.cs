using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;
using PlataformaMulticanalFrontend.Models;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly CatalogoService _catalogoService;

        public ProveedorController(CatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

    }
}
