using RabbitMQ.Client;

namespace VideoProcessingMicroservice.Infrastructure.RabbitMq.Connection;

public interface IRabbitMqConnection
{
    IConnection Connection { get; }
    public Task<IChannel> CreateChannelAsync(
        CancellationToken cancellationToken = default);
}