using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging.Infrastructure.RabbitMq;


namespace Shared.Messaging.Infrastructure.ServiceExtensions
{
    public static class MessagingServiceExtensions
    {
        public static IServiceCollection AddMessaging<TPublisher>(
            this IServiceCollection services) where TPublisher : class, IMessagePublisher
        {
            services.AddSingleton<IMessagePublisher, TPublisher>();
            services.AddHostedService<MessagingStartupHostedService>();
            return services;
        }

    }
}
