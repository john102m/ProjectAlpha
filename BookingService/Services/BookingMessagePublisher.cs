using Shared.Contracts.MessagingBaseClasses;
using Shared.Contracts.MessagingModels;

namespace BookingService.Services
{
    public class BookingMessagePublisher(ILogger<BookingMessagePublisher> logger) : RabbitMqPublisherBase<BookingMessagePublisher>(logger), IMessagePublisher
    {
        private readonly ILogger<BookingMessagePublisher> _logger = logger;
        private readonly string _fanoutExchangeName = "booking-events";
        private readonly string _directExchangeName = "booking-direct";       
        private readonly string _directExchangeRoutingKey = "travel-tips";

        private readonly string _directMessage = "PRIVATE: TEST WHISPER";

        public async Task SetupAsync()
        {
            await InitializeAsync(
                new[]
                {
                    (_fanoutExchangeName, ToRabbitMqType(MessagingExchangeType.Fanout), false),
                    (_directExchangeName, ToRabbitMqType(MessagingExchangeType.Direct), true)
                },
                cancellationToken: default
            );
        }

        public async Task PublishMessageAsync<T>(T message) where T : BaseMessage
        {
            await PublishAsync(_fanoutExchangeName, "", message);

            await PublishAsync(_directExchangeName, _directExchangeRoutingKey, _directMessage);

            _logger.LogInformation("Booking published: {@Booking}", message);
            Console.WriteLine($"Console statement - Booking published: #{message.Id} | {message.Username} | {message.Metadata} | {message.MessageType}");
        }
    }

}
