using Microsoft.AspNetCore.Mvc;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Finalizar Compra";
            
            return View();
        }

        public IActionResult Confirmacion(int? orderId)
        {
            ViewData["Title"] = "Confirmación de Pedido";
            ViewData["OrderId"] = orderId ?? 12345;
            
            return View();
        }
    }
}