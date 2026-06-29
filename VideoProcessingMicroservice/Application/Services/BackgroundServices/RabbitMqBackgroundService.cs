using RabbitMQ.Client;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Infrastructure.RabbitMq.Connection;

namespace VideoProcessingMicroservice.Application.Services.BackgroundServices;

public class RabbitMqBackgroundService : BackgroundService
{
    private readonly IMessageSubscriber _messageSubscriber;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private IChannel? _channel;

    public RabbitMqBackgroundService(IMessageSubscriber messageSubscriber, IRabbitMqConnection rabbitMqConnection)
    {
        _messageSubscriber = messageSubscriber;
        _rabbitMqConnection = rabbitMqConnection;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await _rabbitMqConnection.CreateChannelAsync(cancellationToken);

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
        await _channel!.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}