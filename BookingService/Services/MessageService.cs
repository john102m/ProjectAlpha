using RabbitMQ.Client;
using Shared.Contracts.MessagingModels;
using System.Text.Json;
using System.Text;

namespace BookingService.Services
{
    public class MessageService : IMessageService
    {
        private readonly ILogger<MessageService> _logger;
        private readonly string _rabbitHost;

        public MessageService(ILogger<MessageService> logger)
        {
            _logger = logger;
            _rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        }

        public async Task PublishBookingAsync(BookingMessage booking)
        {
            var factory = new ConnectionFactory { HostName = _rabbitHost };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "booking-confirmed", durable: false, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(booking);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "", routingKey: "booking-confirmed", body: body);

            _logger.LogInformation($"📩 Booking published: #{booking.BookingId} | {booking.Username} | {booking.PackageRef}");
        }

    }
}
