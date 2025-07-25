using Microsoft.AspNetCore.Builder;

namespace Shared.Messaging.Infrastructure.ServiceExtensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Shared.Messaging.Infrastructure.Configuration;

    public static class MessagingConsumerStartupHelper
    {
        public static void AddMessagingConsumerServices<TConsumer>(this WebApplicationBuilder builder)
            where TConsumer : class, IHostedService
        {
            AddMessagingConsumerServicesInternal<TConsumer>(builder.Services);
        }

        public static void AddMessagingConsumerServices<TConsumer>(this HostApplicationBuilder builder)
            where TConsumer : class, IHostedService
        {
            AddMessagingConsumerServicesInternal<TConsumer>(builder.Services);
        }

        private static void AddMessagingConsumerServicesInternal<TConsumer>(IServiceCollection services)
            where TConsumer : class, IHostedService
        {
            var config = ConfigurationReader.Load()
                ?? throw new InvalidOperationException("MessagingConfiguration could not be loaded from XML.");

            services.Configure<MessagingConfiguration>(opts =>
            {
                opts.Exchanges = config.Exchanges;
                opts.Queues = config.Queues;
                opts.RoutingKeys = config.RoutingKeys;
            });

            services.AddHostedService<TConsumer>();
        }
    }

}
