namespace Shared.Messaging.Infrastructure.Models
{
    /// <summary>
    /// Represents a single exchange binding configuration.
    /// </summary>
    public record ExchangeBinding(string QueueName, string ExchangeName, string RoutingKey);

    /// <summary>
    /// Wrapper class to group multiple ExchangeBinding records.
    /// Ideal for config-driven or dynamic queue binding setups.
    /// </summary>
    public class ExchangeBindingType
    {
        /// <summary>
        /// List of bindings representing exchange-to-queue relationships.
        /// </summary>
        public List<ExchangeBinding> Bindings { get; set; } = new();

        /// <summary>
        /// Optional description or tag for the configuration (useful in logs or multi-service setups).
        /// </summary>
        public string ConfigLabel { get; set; } = string.Empty;

        /// <summary>
        /// Optional exchange type hint if you're grouping by behavior.
        /// </summary>
        public MessagingExchangeType ExchangeType { get; set; } = MessagingExchangeType.Fanout;
    }
}
