using Microsoft.Extensions.Options;
using Shared.Messaging.Infrastructure.Configuration;
using Shared.Messaging.Infrastructure.RabbitMq;

namespace TravelTipsService;

public class WorkerConsumer : RabbitMqConsumerBase<BaseMessage, WorkerConsumer>
{
    private readonly ILogger<WorkerConsumer> _logger;
    private readonly MessagingConfiguration _config;

    private readonly string _travelTipsQueueName;
    private readonly string _bookingExchangeName;
    private readonly string _directExchangeName;
    private readonly string _directExchangeRoutingKey;

    public WorkerConsumer(
        ILogger<WorkerConsumer> logger,
        IOptions<MessagingConfiguration> options
    ) : base(logger)
    {
        _logger = logger;
        _config = options.Value;

        _travelTipsQueueName = _config.Queues.BookingTips;
        _bookingExchangeName = _config.Exchanges.BookingExchange;
        _directExchangeName = _config.Exchanges.DirectExchange;
        _directExchangeRoutingKey = _config.RoutingKeys.TravelTips;
    }

    protected override Task HandleMessageAsync(BaseMessage message)
    {
        _logger.LogInformation(
            "Worker received booking #{MessageId} for {Username} → {Metadata} | {MessageType}",
            message.Id, message.Username, message.Metadata, message.MessageType
        );
        return Task.CompletedTask;
    }

    protected override Task HandleStringMessageAsync(string message)
    {
        _logger.LogInformation("{Consumer} received a string: {Message}", nameof(WorkerConsumer), message);
        return Task.CompletedTask;
    }

    protected override List<ExchangeBinding> GetBindings() =>
    [
        new(_travelTipsQueueName, _bookingExchangeName, ""),
        new(_travelTipsQueueName, _directExchangeName, _directExchangeRoutingKey)
    ];
}
