namespace Shared.Contracts.MessagingModels
{
    public class BookingMessage
    {
        public string BookingId { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;
    }
}
