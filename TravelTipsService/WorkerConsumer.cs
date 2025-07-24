using Shared.Contracts.MessagingBaseClasses;
using Shared.Contracts.MessagingModels;

namespace TravelTipsService;

public class WorkerConsumer(ILogger<WorkerConsumer> logger) : RabbitMqConsumerBase<BaseMessage, WorkerConsumer>(logger, _queueName, _exchangeName)
{
    private static readonly string _queueName = "booking-tips";
    private static readonly string _exchangeName = "booking-events";
    private static readonly string _directExchangeName = "booking-direct";

    private readonly ILogger<WorkerConsumer> _logger = logger;

    protected override Task HandleMessageAsync(BaseMessage message)
    {
        _logger.LogInformation($"Worker received booking #{message.Id} for {message.Username} → {message?.Metadata} | {message?.MessageType}");
        return Task.CompletedTask;
    }

    protected override Task HandleStringMessageAsync(string message)  // or error
    {
        _logger.LogInformation($"A string was received: {message}");
        return Task.CompletedTask;
    }

    protected override async Task BindQueueAsync(CancellationToken cancellationToken)
    {
        if (Channel is null)
            throw new InvalidOperationException("RabbitMQ channel is null during queue binding.");

        await Channel.QueueDeclareAsync(_queueName, durable: false, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await Channel.QueueBindAsync(_queueName, _exchangeName, routingKey: "", cancellationToken: cancellationToken);
        await Channel.QueueBindAsync(_queueName, _directExchangeName, "travel-tips", cancellationToken: cancellationToken);

        _logger.LogInformation("Queue '{Queue}' bound to exchange '{Exchange}'", _queueName, _exchangeName);
        _logger.LogInformation("Queue '{Queue}' bound to exchange '{Exchange}'", _queueName, _directExchangeName);
        await Task.CompletedTask;
    }
}
