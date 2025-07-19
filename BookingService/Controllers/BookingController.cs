using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Shared.Contracts.MessagingModels;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;

        public BookingController(ILogger<BookingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("health")]
        public IActionResult Get() => Ok("BookingService is running!");

        [HttpPost("book")]
        public async Task<IActionResult> PostBookingAsync()
        {
            var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var factory = new ConnectionFactory() { HostName = rabbitHost };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "booking-confirmed", durable: false, exclusive: false, autoDelete: false);

            var booking = new BookingMessage
            {
                BookingId = "123",
                UserEmail = "alice@example.com",
                RoomType = "Deluxe Suite"
            };

            var json = JsonSerializer.Serialize(booking);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "", routingKey: "booking-confirmed", body: body);
            Debug.WriteLine($"📩 Booking confirmed and message sent. #{booking?.BookingId} | {booking?.UserEmail} | {booking?.RoomType}");
            _logger.LogInformation($"📩 Booking confirmed and message sent. #{booking?.BookingId} | {booking?.UserEmail} | {booking?.RoomType}");
            return Ok("Booking confirmed and message sent.");
        }
    }
}
