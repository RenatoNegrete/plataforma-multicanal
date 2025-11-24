using System;
using System.Collections.Generic;

namespace ProviderData.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int Precio { get; set; }

    public string Imagen { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public int Stock { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
