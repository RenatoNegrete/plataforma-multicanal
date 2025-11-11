using System;

namespace ProviderAPI.DTOs;

public class OrderBatchRequest
{
    public string OrderId { get; set; } = null!;
    public string CustomerMail { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public List<OrderItemDTO>? Items { get; set; }
}
