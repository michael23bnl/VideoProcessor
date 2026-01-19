using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Application.DTO;

namespace VideoProcessingMicroservice.Infrastructure.RabbitMq;

public class RabbitMqSubscriber : IMessageSubscriber
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqSubscriber(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ReceiveMessageAsync(IChannel channel, CancellationToken cancellationToken)
    {
        var queue = await channel.QueueDeclareAsync(
            queue: "video-processing-queue",
            durable: true,
            autoDelete: false,
            exclusive: false,
            cancellationToken: cancellationToken);
        
        await channel.BasicQosAsync(
            prefetchSize: 0, 
            prefetchCount: 1, 
            global: false,
            cancellationToken: cancellationToken);
        
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<VideoUploadedEvent>(messageJson);
            
            using var scope = _scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IVideoUploadedHandler>();

            await handler.HandleAsync(message.Key, message.Id, cancellationToken);
            await channel.BasicAckAsync(
                deliveryTag: ea.DeliveryTag, 
                multiple: false,
                cancellationToken: cancellationToken);
        };

        await channel.BasicConsumeAsync(
            queue: queue.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: cancellationToken);
    }
}