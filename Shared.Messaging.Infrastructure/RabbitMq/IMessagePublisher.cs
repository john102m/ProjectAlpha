
using Shared.Messaging.Infrastructure.Models;

namespace Shared.Messaging.Infrastructure.RabbitMq
{
    public interface IMessagePublisher
    {
        Task PublishMessageAsync<TMessage>(TMessage message) where TMessage : BaseMessage;
        Task SetupAsync();
    }
}

