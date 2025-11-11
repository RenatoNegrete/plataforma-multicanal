using System;
using System.Text.Json;
using Confluent.Kafka;

namespace ProviderAPI.Services;

public class KafkaProducerService
{
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService(IConfiguration config)
    {
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"]
        };

        _topic = config["Kafka:Topic"];
        _producer = new ProducerBuilder<Null, string>(kafkaConfig).Build();
    }   

    public async Task PublishAsync(object message)
    {
        var jsonMessage = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = jsonMessage });
    }
}
