using System.Xml.Linq;

namespace Shared.Messaging.Infrastructure.Configuration
{

    public static class ConfigurationReader
    {
        private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "configuration.xml");

        public static MessagingConfiguration Load()
        {
            if (!File.Exists(ConfigPath))
                throw new FileNotFoundException($"Config file not found at '{ConfigPath}'.");

            var doc = XDocument.Load(ConfigPath);

            return new MessagingConfiguration
            {
                Exchanges = new Exchanges
                {
                    BookingExchange = doc.Root?.Element("Exchanges")?.Element("BookingExchange")?.Value ?? string.Empty,
                    DirectExchange = doc.Root?.Element("Exchanges")?.Element("DirectExchange")?.Value ?? string.Empty,
                    UserExchange = doc.Root?.Element("Exchanges")?.Element("UserExchange")?.Value ?? string.Empty
                },
                Queues = new Queues
                {
                    BookingEmails = doc.Root?.Element("Queues")?.Element("BookingEmails")?.Value ?? string.Empty,
                    BookingTips = doc.Root?.Element("Queues")?.Element("BookingTips")?.Value ?? string.Empty
                },
                RoutingKeys = new RoutingKeys
                {
                    TravelTips = doc.Root?.Element("RoutingKeys")?.Element("TravelTips")?.Value ?? string.Empty
                }
            };
        }
    }
}
