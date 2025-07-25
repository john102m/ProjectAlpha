namespace Shared.Messaging.Infrastructure.RabbitMq
{
    public class BaseMessage
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty; // e.g. "booking.created", "user.registered"
        public string Metadata { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
