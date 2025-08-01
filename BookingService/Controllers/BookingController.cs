﻿using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.MessagingBaseClasses;
using Shared.Contracts.MessagingModels;

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
            var reservations = await _repo.GetEnrichedReservationsAsync();
            return Ok(reservations);
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
