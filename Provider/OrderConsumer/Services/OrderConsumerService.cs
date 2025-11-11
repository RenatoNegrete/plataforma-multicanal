using System;
using System.Text;
using System.Text.Json;

namespace OrderConsumer.Services;

public class OrderConsumerService
{
    private readonly ILogger<OrderConsumerService> _logger;
    private readonly EmailService _emailService;

    public OrderConsumerService(ILogger<OrderConsumerService> logger, EmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task HandleOrderReceivedAsync(string message)
    {
        var orderEvent = JsonSerializer.Deserialize<OrderEvent>(message);

        var subject = "Orden recibida";
        var body = new StringBuilder();
        body.AppendLine($"Hola, se ha recibido una orden a nombre de {orderEvent.CustomerEmail}");
        body.AppendLine($"Fecha: {orderEvent.Timestamp}");
        body.AppendLine();
        body.AppendLine("Detalle de la orden");
        body.AppendLine("--------------------------------");
        body.AppendLine($"En la orden hay {orderEvent.ItemsCount} items");
        foreach (var item in orderEvent.Items)
        {
            body.AppendLine($"{item.ProductName} (x{item.Quantity}) - ${item.SubTotal}");
        }
        body.AppendLine("--------------------------------------");
        body.AppendLine($"Total: ${orderEvent.Total}");
        await _emailService.SendEmailAsync(orderEvent.CustomerEmail, subject, body.ToString());
    }
}

public class OrderEvent
{
    public string CustomerEmail { get; set; }
    public int Total { get; set; }
    public int ItemsCount { get; set; }
    public List<OrderItem> Items { get; set; }
    public DateTime Timestamp { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
    public int SubTotal { get; set; }
}