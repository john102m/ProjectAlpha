
using Shared.Contracts.MessagingBaseClasses;
using Shared.Contracts.MessagingModels;

namespace UserService.Services
{
    public class UserMessagePublisher : IMessagePublisher
    {
        public Task SetupAsync()
        {
            throw new NotImplementedException();
        }
        public Task PublishMessageAsync<TMessage>(TMessage message) where TMessage : BaseMessage
        {
            throw new NotImplementedException();
        }
    }
}
