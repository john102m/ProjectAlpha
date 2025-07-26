namespace Shared.Messaging.Infrastructure.MessageModels
{
    public class MessageEnvelope<T>
    {
        public T Payload { get; set; } = default!;
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string SourceService { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
    }
}
