using System;

namespace OrderConsumer.Services;

public class KafkaConsumerHostedService : BackgroundService
{
    private readonly KafkaConsumerService _consumer;

    public KafkaConsumerHostedService(KafkaConsumerService consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.StartAsync(stoppingToken);
    }
}
