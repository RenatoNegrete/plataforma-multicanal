using System;
using System.Collections.Generic;

namespace ProviderData.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string BuyerEmail { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Status { get; set; }

    public DateTime? OrderDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}
