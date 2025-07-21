using RabbitMQ.Client;
using Shared.Contracts.MessagingModels;
using System.Text;
using System.Text.Json;

namespace BookingService.Services
{
    public class MessageService : IMessageService, IDisposable
    {
        private readonly ILogger<MessageService> _logger;
        private readonly string _rabbitHost;
        private IConnection? _connection;
        private IChannel? _channel;

        public MessageService(ILogger<MessageService> logger)
        {
            _logger = logger;
            _rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        }

        public async Task SetupAsync()
        {
            
            var factory = new ConnectionFactory { HostName = _rabbitHost };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("booking-events", ExchangeType.Fanout, durable: false);
            await _channel.ExchangeDeclareAsync("booking-direct", ExchangeType.Direct, durable: true);

            //await channel.QueueDeclareAsync(queue: "booking-confirmed", durable: false, exclusive: false, autoDelete: false);

        }

        public async Task PublishBookingAsync(BookingMessage booking)
        {

            if (_channel == null)
            {
                _logger.LogError("RabbitMQ channel is not initialized.");
                throw new InvalidOperationException("Cannot publish—channel not set up.");
            }

            var json = JsonSerializer.Serialize(booking);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel!.BasicPublishAsync(
                exchange: "booking-events", 
                routingKey: "", 
                body: body
            );

            // Direct: only TravelTips hears it
            await _channel!.BasicPublishAsync(
                exchange: "booking-direct",
                routingKey: "travel-tips",
                body: Encoding.UTF8.GetBytes("TEST WHISPER")
            );

            _logger.LogInformation($"📩 Booking published: #{booking.BookingId} | {booking.Username} | {booking.Metadata} | {booking.PackageRef}");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

    }
}
