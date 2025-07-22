using Shared.Contracts.MessagingModels;

namespace EmailService.Services
{
    public class EmailConsumer(ILogger<EmailConsumer> logger) : RabbitMqConsumerBase<BookingMessage, EmailConsumer>(logger, _queueName, _exchangeName)
    {
        private static readonly string _queueName = "booking-emails";
        private static readonly string _exchangeName = "booking-events";
        private readonly ILogger<EmailConsumer> _logger = logger;

        protected override Task HandleMessageAsync(BookingMessage message)
        {
            _logger.LogInformation($"Emailing booking #{message.BookingId} for {message.Username} → {message?.Metadata} | {message?.PackageRef}");
            return Task.CompletedTask;
        }

        protected override Task HandleStringMessageAsync(string message)
        {
            _logger.LogInformation($"A string was received: {message}");
            return Task.CompletedTask;
        }
    }

}
