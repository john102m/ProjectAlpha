using Shared.Contracts.MessagingBaseClasses;
using Shared.Contracts.MessagingModels;

namespace EmailService.Services
{
    public class EmailConsumer(ILogger<EmailConsumer> logger) : RabbitMqConsumerBase<BaseMessage, EmailConsumer>(logger, _queueName, _exchangeName)
    {
        private static readonly string _queueName = "booking-emails";
        private static readonly string _exchangeName = "booking-events";
        private readonly ILogger<EmailConsumer> _logger = logger;

        protected override Task HandleMessageAsync(BaseMessage message)
        {
            _logger.LogInformation($"Emailing booking #{message.Id} for {message.Username} → {message?.Metadata} | {message?.MessageType}");
            return Task.CompletedTask;
        }

        protected override Task HandleStringMessageAsync(string message)
        {
            _logger.LogInformation($"A string was received: {message}");
            return Task.CompletedTask;
        }
    }

}
