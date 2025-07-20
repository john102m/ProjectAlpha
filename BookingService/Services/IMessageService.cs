using Shared.Contracts.MessagingModels;

namespace BookingService.Services
{
    public interface IMessageService
    {
        Task PublishBookingAsync(BookingMessage booking);
    }
}
