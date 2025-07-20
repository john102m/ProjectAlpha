using BookingService.Models;
using BookingService.Services;
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
        private readonly IBookingRepository _repo;

        public BookingController(ILogger<BookingController> logger,IBookingRepository repo)
        {
            _logger = logger;
            _repo = repo;
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


        [HttpGet("reservations")]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _repo.GetReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("reservations/{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _repo.GetReservationByIdAsync(id);
            return reservation != null ? Ok(reservation) : NotFound();
        }

        [HttpPost("reservations")]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            var created = await _repo.CreateReservationAsync(reservation);
            return CreatedAtAction(nameof(GetReservation), new { id = created.Id }, created);
        }

        [HttpPut("reservations/{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation reservation)
        {
            var updated = await _repo.UpdateReservationAsync(id, reservation);
            return updated ? Ok("Reservation updated") : NotFound();
        }

        [HttpDelete("reservations/{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var deleted = await _repo.DeleteReservationAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("reservations/packageinfo")]
        public async Task<IActionResult> GetEnrichedReservationsAsync()
        {
            var reservations = await _repo.GetEnrichedReservationsAsync();
            return Ok(reservations);
        }
        [HttpGet("reservations/search")]
        public async Task<IActionResult> SearchReservationsByGuest([FromQuery] string guestName)
        {
            if (string.IsNullOrWhiteSpace(guestName))
                return BadRequest("Guest name must be provided.");

            var matches = await _repo.SearchReservationsAsync(guestName);
            return Ok(matches);
        }


    }
}
