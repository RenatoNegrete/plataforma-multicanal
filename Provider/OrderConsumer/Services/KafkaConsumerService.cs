using System;
using Confluent.Kafka;

namespace OrderConsumer.Services;

public class KafkaConsumerService
{
    private readonly string _bootstrapServers = "kafka:9092";
    private readonly string _topic = "order-confirmations";
    private readonly string _groupId = "provider-consumer-group";
    private readonly OrderConsumerService _orderService;

    public KafkaConsumerService(OrderConsumerService orderService)
    {
        _orderService = orderService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topic);
        Console.WriteLine($"🟢 Listening to topic: {_topic}");
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(cancellationToken);
                    Console.WriteLine($"📩 Message received: {cr.Message.Value}");
                    await _orderService.HandleOrderReceivedAsync(cr.Message.Value);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"⚠️ Error: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("🛑 Consumer stopping...");
            consumer.Close();
        }
    }
}