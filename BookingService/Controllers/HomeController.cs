using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BookingService.Controllers
{
    [ApiController]
    //[Route("[controller]")]    
    //[Route("service")]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("BookingService is running!");


        [HttpPost("book")]
        public async Task<IActionResult> PostBookingAsync()
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };

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

            return Ok("Booking confirmed and message sent.");
        }
    }
}
