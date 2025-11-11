using System;
using System.Collections.Generic;

namespace ProviderData.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Price { get; set; }

    public int Quantity { get; set; }

    public int PriceType { get; set; }

    public int QuantityType { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
