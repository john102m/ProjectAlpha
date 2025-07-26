using Microsoft.Extensions.Options;
using Shared.Messaging.Infrastructure.Configuration;
using Shared.Messaging.Infrastructure.Models;
using Shared.Messaging.Infrastructure.RabbitMq;


namespace BookingService.Services
{
    public class BookingMessagePublisher : RabbitMqPublisherBase<BookingMessagePublisher>, IMessagePublisher
    {
        private readonly ILogger<BookingMessagePublisher> _logger;
        private readonly MessagingConfiguration _config;

        private readonly string _bookingExchangeName;
        private readonly string _directExchangeName;
        private readonly string _directExchangeRoutingKey;
        private readonly string _directMessage;

        public BookingMessagePublisher(
            ILogger<BookingMessagePublisher> logger,
            IOptions<MessagingConfiguration> options
        ) : base(logger)
        {
            _logger = logger;
            _config = options.Value;

            _bookingExchangeName = _config.Exchanges.BookingExchange;
            _directExchangeName = _config.Exchanges.DirectExchange;
            _directExchangeRoutingKey = _config.RoutingKeys.TravelTips;
            _directMessage = "PRIVATE: TEST WHISPER"; // Or pull this from config as well!
        }

        public async Task SetupAsync()
        {
            await InitializeAsync(
                new[]
                {
                    (_bookingExchangeName, ToRabbitMqType(MessagingExchangeType.Fanout), false),
                    (_directExchangeName, ToRabbitMqType(MessagingExchangeType.Direct), true)
                },
                cancellationToken: default
            );
        }

        public async Task PublishMessageAsync<T>(T message) where T : BaseMessage
        {
            await PublishAsync(_bookingExchangeName, "", message);
            await PublishAsync(_directExchangeName, _directExchangeRoutingKey, _directMessage);

            _logger.LogInformation("Booking message published: {@Message} on Exchange {@Exchange}", message, _bookingExchangeName);
            _logger.LogInformation("Also published direct: {@Message} on Exchange {@Exchange}", _directMessage, _directExchangeName);

            Console.WriteLine($"Console statement - Booking message published: #{message.Id} | {message.Username} | {message.Metadata} | {message.MessageType}");
        }
    }
}
