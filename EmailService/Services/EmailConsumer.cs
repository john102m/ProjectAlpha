using Microsoft.Extensions.Options;
using Shared.Messaging.Infrastructure.Configuration;
using Shared.Messaging.Infrastructure.Models;
using Shared.Messaging.Infrastructure.RabbitMq;

namespace EmailService.Services
{
    public class EmailConsumer : RabbitMqConsumerBase<BaseMessage, EmailConsumer>
    {
        private readonly ILogger<EmailConsumer> _logger;
        private readonly MessagingConfiguration _config;

        private readonly string _emailQueueName;
        private readonly string _bookingExchangeName;
        private readonly string _userExchangeName;

        public EmailConsumer(
            ILogger<EmailConsumer> logger,
            IOptions<MessagingConfiguration> options
        ) : base(logger)
        {
            _logger = logger;
            _config = options.Value;

            _emailQueueName = _config.Queues.BookingEmails;
            _bookingExchangeName = _config.Exchanges.BookingExchange;
            _userExchangeName = _config.Exchanges.UserExchange;
        }

        protected override Task HandleMessageAsync(BaseMessage message)
        {
            _logger.LogInformation(
                "Email consumer received message: #{MessageId} for {Username} → {Metadata} | {MessageType}",
                message.Id, message.Username, message.Metadata, message.MessageType
            );
            return Task.CompletedTask;
        }

        protected override Task HandleStringMessageAsync(string message)
        {
            _logger.LogInformation("{Consumer} received a string: {Message}", nameof(EmailConsumer), message);
            return Task.CompletedTask;
        }

        protected override List<ExchangeBinding> GetBindings() =>
        [
            new(_emailQueueName, _bookingExchangeName, ""),
            new(_emailQueueName, _userExchangeName, "")
        ];
    }
}
