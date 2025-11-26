using System;

namespace PlataformaMulticanalFrontend.Models;

public class AgregarProductoDto
{
    public string ProductoId { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Cantidad { get; set; } = 1;
    public string ImagenUrl { get; set; }
}
