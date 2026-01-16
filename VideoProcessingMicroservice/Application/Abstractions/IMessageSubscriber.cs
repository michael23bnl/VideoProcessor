using RabbitMQ.Client;

namespace VideoProcessingMicroservice.Application.Abstractions;

public interface IMessageSubscriber
{
    Task ReceiveMessageAsync(IChannel channel, CancellationToken cancellationToken);
}