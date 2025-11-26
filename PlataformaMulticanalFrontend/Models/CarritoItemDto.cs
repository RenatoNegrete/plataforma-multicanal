using System;

namespace PlataformaMulticanalFrontend.Models;

public class CarritoItemDto
{
    public string ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string ImagenUrl { get; set; }
}
