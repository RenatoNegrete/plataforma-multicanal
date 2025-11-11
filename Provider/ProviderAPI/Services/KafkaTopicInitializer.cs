using System;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace ProviderAPI.Services;

public class KafkaTopicInitializer
{
    private readonly string _bootstrapServers = "kafka:9092";
    public async Task EnsureTopicExistsAsync(string topicName)
    {
        var config = new AdminClientConfig { BootstrapServers = _bootstrapServers };
        using var adminClient = new AdminClientBuilder(config).Build();
        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            bool exists = metadata.Topics.Any(t => t.Topic == topicName && t.Error.Code == ErrorCode.NoError);
            if (!exists)
            {
                Console.WriteLine($"Topic '{topicName}' not found. Creating...");
                var topicSpec = new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = 3,
                    ReplicationFactor = 1
                };
                await adminClient.CreateTopicsAsync(new List<TopicSpecification> { topicSpec });
                Console.WriteLine($"Topic '{topicName}' created successfully!");
            }
            else
            {
                Console.WriteLine($"Topic '{topicName}' already exists.");
            }
        }
        catch (CreateTopicsException ex)
        {
            Console.WriteLine($"Error creating topic: {ex.Results[0].Error.Reason}");
        }
        catch (KafkaException ex)
        {
            Console.WriteLine($"Kafka not reachable: {ex.Message}");
        }
    }
}
