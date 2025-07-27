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

        public BookingController(ILogger<BookingController> logger, IBookingRepository repo, IMessagePublisher messageService)
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
            try
            {
                var reservationViews = await _repo.GetReservationsAsync();
                return Ok(reservationViews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reservations");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("reservations/{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            try
            {
                var reservationView = await _repo.GetReservationByIdAsync(id);
                if (reservationView == null)
                    return NotFound();

                return Ok(reservationView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reservation with id {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("reservations")]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            try
            {

                var created = await _repo.CreateReservationAsync(reservation);
                var message = new BaseMessage
                {
                    Id = reservation.Id,
                    Username = reservation.GuestName,
                    Metadata = reservation.ExtraInfo,
                    MessageType = reservation.PackageId.ToString(),
                    CreatedAt = DateTime.Now,
                };
                _ = _messageService.PublishMessageAsync(message);


                return CreatedAtAction(nameof(GetReservation), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("reservations/{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation reservation)
        {
            try
            {

                var updated = await _repo.UpdateReservationAsync(id, reservation);
                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reservation with id {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("reservations/{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var deleted = await _repo.DeleteReservationAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reservation with id {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("reservations/packageinfo")]
        public async Task<IActionResult> GetEnrichedReservationsAsync([FromQuery] string? id)
        {
            try
            {
                var enriched = await _repo.GetEnrichedReservationsAsync();
                if (enriched == null)
                    return NotFound();

                return Ok(enriched);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to enriched reservations");
                return StatusCode(403, "Permission denied.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching enriched reservations");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("reservations/packageinfo/{id}")]
        public async Task<IActionResult> GetEnrichedReservationsByIdAsync(int id)
        {
            try
            {
                var single = await _repo.GetEnrichedReservationsByIdAsync(id);
                if (single == null)
                    return NotFound();
                return Ok(single);

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to enriched reservations");
                return StatusCode(403, "Permission denied.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching enriched reservations");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("reservations/search")]
        public async Task<IActionResult> SearchReservationsByGuest([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Guest name must be provided.");

            try
            {
                var results = await _repo.SearchReservationsAsync(searchTerm);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching reservations");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
