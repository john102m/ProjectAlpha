using BookingService.Models;

namespace BookingService.Services
{
    public interface IBookingRepository
    {
        Task<IEnumerable<ReservationView>> GetReservationsAsync();
        Task<ReservationView?> GetReservationByIdAsync(int id);
        Task<Reservation> CreateReservationAsync(Reservation reservation);
        Task<bool> UpdateReservationAsync(int id, Reservation reservation);
        Task<bool> DeleteReservationAsync(int id);
        Task<IEnumerable<BookingWithPackageDto>> GetEnrichedReservationsAsync();
        Task<BookingWithPackageDto> GetEnrichedReservationsByIdAsync(int id);
        Task<IEnumerable<BookingWithPackageDto>> SearchReservationsAsync(string guestName);
    }
}
