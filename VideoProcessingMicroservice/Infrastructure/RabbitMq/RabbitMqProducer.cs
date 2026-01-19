using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Application.DTO;
using VideoProcessingMicroservice.Infrastructure.RabbitMq.Connection;

namespace VideoProcessingMicroservice.Infrastructure.RabbitMq;

public class RabbitMqProducer : IMessageProducer
{
    private readonly IChannel _channel;

    private RabbitMqProducer(IChannel channel)
    {
        _channel = channel;
    }
    
    public static async Task<RabbitMqProducer> CreateAsync(
        IRabbitMqConnection connection,
        CancellationToken cancellationToken)
    {
        var channel = await connection.CreateChannelAsync(cancellationToken);

        return new RabbitMqProducer(channel);
    }

    public async Task SendMessageAsync(VideoUploadedEvent message, CancellationToken cancellationToken)
    {
        var queue = await _channel.QueueDeclareAsync(
            queue: "video-processing-queue", 
            durable: true, 
            autoDelete: false, 
            exclusive: false, 
            cancellationToken: cancellationToken);
        
        var messageJson = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageJson);
        
        var properties = new BasicProperties
        {
            Persistent = true
        };

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue.QueueName,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}