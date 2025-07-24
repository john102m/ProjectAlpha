
using Shared.Contracts.MessagingModels;

namespace Shared.Contracts.MessagingBaseClasses
{
    public interface IMessagePublisher
    {
        Task PublishMessageAsync<TMessage>(TMessage message) where TMessage : BaseMessage;
        Task SetupAsync();
    }
}
