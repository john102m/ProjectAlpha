using BookingService.Models;

namespace BookingService.Services
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Reservation>> GetReservationsAsync();
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task<Reservation> CreateReservationAsync(Reservation reservation);
        Task<bool> UpdateReservationAsync(int id, Reservation reservation);
        Task<bool> DeleteReservationAsync(int id);
        Task<IEnumerable<BookingWithPackageDto>> GetEnrichedReservationsAsync();
        Task<IEnumerable<BookingWithPackageDto>> SearchReservationsAsync(string guestName);
    }
}
