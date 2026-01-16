using RabbitMQ.Client;

namespace VideoProcessingMicroservice.Infrastructure.RabbitMq.Connection;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly IConnection? _connection;

    public IConnection Connection => _connection!;

    private RabbitMqConnection(IConnection connection)
    {
        _connection = connection;
    }

    public static async Task<RabbitMqConnection> InitializeConnectionAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
        };
        var connection = await factory.CreateConnectionAsync();
        
        return new RabbitMqConnection(connection);
    }
    
    public Task<IChannel> CreateChannelAsync(
        CancellationToken cancellationToken = default)
        => _connection.CreateChannelAsync(options: null, cancellationToken: cancellationToken);

    public void Dispose()
    {
        _connection?.Dispose();
    }
}