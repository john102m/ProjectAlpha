namespace Shared.Contracts.MessagingModels
{
    public class BookingMessage
    {
        public int BookingId { get; set; }

        public string Username { get; set; } = string.Empty;

        public int PackageRef { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
