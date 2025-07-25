
namespace Shared.Messaging.Infrastructure.Configuration
{

    public class MessagingConfiguration
    {
        public Exchanges Exchanges { get; set; } = new();
        public Queues Queues { get; set; } = new();
        public RoutingKeys RoutingKeys { get; set; } = new();
    }

    public class Exchanges
    {
        public string BookingExchange { get; set; } = string.Empty;
        public string DirectExchange { get; set; } = string.Empty;
        public string UserExchange { get; set; } = string.Empty;
    }

    public class Queues
    {
        public string BookingEmails { get; set; } = string.Empty;
        public string BookingTips { get; set; } = string.Empty;
    }

    public class RoutingKeys
    {
        public string TravelTips { get; set; } = string.Empty;
    }

}
