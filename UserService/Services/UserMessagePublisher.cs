using Microsoft.Extensions.Options;
using Shared.Messaging.Infrastructure.Configuration;
using Shared.Messaging.Infrastructure.RabbitMq;


namespace UserService.Services
{
    public class UserMessagePublisher : RabbitMqPublisherBase<UserMessagePublisher>, IMessagePublisher
    {
        private readonly ILogger<UserMessagePublisher> _logger;
        private readonly MessagingConfiguration _config;

        private readonly string _userExchangeName;

        public UserMessagePublisher(
            ILogger<UserMessagePublisher> logger,
            IOptions<MessagingConfiguration> options
        ) : base(logger)
        {
            _logger = logger;
            _config = options.Value;

            _userExchangeName = _config.Exchanges.UserExchange;
        }

        public async Task SetupAsync()
        {
            await InitializeAsync(
                new[]
                {
                    (_userExchangeName, ToRabbitMqType(MessagingExchangeType.Fanout), false)
                },
                cancellationToken: default
            );
        }

        public async Task PublishMessageAsync<TMessage>(TMessage message) where TMessage : BaseMessage
        {
            await PublishAsync(_userExchangeName, "", message);

            _logger.LogInformation("User message published: {@Message} on Exchange {@Exchange}", message, _userExchangeName);
        }
    }
}
