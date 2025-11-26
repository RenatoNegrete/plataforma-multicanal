using System;

namespace PlataformaMulticanalFrontend.Models;

public class CarritoDto
{
    public List<CarritoItemDto> Items { get; set; } = new List<CarritoItemDto>();

        public decimal CalcularSubtotal()
        {
            return Items.Sum(i => i.Precio * i.Cantidad);
        }
}
