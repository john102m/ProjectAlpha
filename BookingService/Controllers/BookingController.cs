using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Messaging.Infrastructure.Models;
using Shared.Messaging.Infrastructure.RabbitMq;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;        
        private readonly IBookingRepository _repo;
        private readonly IMessagePublisher _messageService;

        public BookingController(ILogger<BookingController> logger,IBookingRepository repo, IMessagePublisher messageService)
        {
            _logger = logger;
            _repo = repo;
            _messageService = messageService;
        }

        [HttpGet("health")]
        public IActionResult Get() => Ok("BookingService is running!");

        [HttpPost("book")]
        public async Task<IActionResult> PostBookingAsync()
        {

            var message = new BaseMessage
            {
                Id = 123,
                Username = "Test User Bobby",
                MessageType = "12",
                CreatedAt = DateTime.Now,
            };

            await _messageService.PublishMessageAsync(message);
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
            var message = new BaseMessage
            {
                Id = reservation.Id,
                Username = reservation.GuestName,
                Metadata = reservation.Package,
                MessageType = reservation.PackageId.ToString(),
                CreatedAt = DateTime.Now,
            };
            _ = _messageService.PublishMessageAsync(message);

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
            return deleted ? Ok($"Reservation Id: {id} deleted") : NotFound();
        }

        [HttpGet("reservations/packageinfo")]
        public async Task<IActionResult> GetEnrichedReservationsAsync()
        {
            try
            {
                var reservations = await _repo.GetEnrichedReservationsAsync();
                return Ok(reservations);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Forbidden access to reservations view.");
                return StatusCode(403, "Access forbidden: insufficient permissions.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetEnrichedReservationsAsync.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet("reservations/search")]
        public async Task<IActionResult> SearchReservationsByGuest([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Guest name must be provided.");

            var matches = await _repo.SearchReservationsAsync(searchTerm);
            return Ok(matches);
        }
    }
}
