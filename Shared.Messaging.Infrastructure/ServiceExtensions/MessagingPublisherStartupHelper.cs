using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging.Infrastructure.Configuration;
using Shared.Messaging.Infrastructure.RabbitMq;


namespace Shared.Messaging.Infrastructure.ServiceExtensions
{
    public static class MessagingPublisherStartupHelper
    {
        public static void AddMessagingPublisherServices<TPublisher>(this WebApplicationBuilder builder)
            where TPublisher : class, IMessagePublisher
        {
            // Load XML-based config (from a shared config file, e.g., Configuration.xml)
            var config = ConfigurationReader.Load() ?? throw new InvalidOperationException("MessagingConfiguration could not be loaded from XML.");

            // Register the loaded config into the DI container
            builder.Services.Configure<MessagingConfiguration>(opts =>
            {
                opts.Exchanges = config.Exchanges;
                opts.Queues = config.Queues;
                opts.RoutingKeys = config.RoutingKeys;
            });

            // Register the messaging services using the provided TPublisher
            builder.Services.AddMessaging<TPublisher>();
        }
    }
}
