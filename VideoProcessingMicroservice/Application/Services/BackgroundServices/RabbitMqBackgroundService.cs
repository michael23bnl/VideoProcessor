using RabbitMQ.Client;
using VideoProcessingMicroservice.Application.Abstractions;

namespace VideoProcessingMicroservice.Application.Services.BackgroundServices;

public class RabbitMqBackgroundService : BackgroundService
{
    private readonly IMessageSubscriber _messageSubscriber;
    private IChannel? _channel;
    private IConnection? _connection;

    public RabbitMqBackgroundService(IMessageSubscriber messageSubscriber)
    {
        _messageSubscriber = messageSubscriber;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(null, cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _messageSubscriber.ReceiveMessageAsync(_channel!, cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel!.CloseAsync(cancellationToken);
        await _connection!.CloseAsync(cancellationToken);

        await _channel!.DisposeAsync();
        await _connection!.DisposeAsync();
        
        await base.StopAsync(cancellationToken);
    }
}