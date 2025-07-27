using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Shared.Messaging.Infrastructure.MessageModels;
using Shared.Messaging.Infrastructure.Models;
using System.Text;
using System.Text.Json;

namespace Shared.Messaging.Infrastructure.RabbitMq
{


    public abstract class RabbitMqConsumerBase<TPayload, TConsumer>(
        ILogger<TConsumer> logger) : BackgroundService
    {
        private readonly ILogger<TConsumer> _logger = logger;
        private readonly string _rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private IConnection? _connection = null;
        protected IChannel? Channel { get; private set; }
        protected AsyncEventingBasicConsumer? Consumer { get; private set; }

        /// <summary>
        /// Provides exchange binding definitions for the consumer's queue.
        /// Override in consumer class to declare which exchanges this queue should bind to.
        /// </summary>
        protected abstract List<ExchangeBinding> GetBindings();


        /// <summary>
        /// Handles deserialized messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <param name="message">The strongly typed message object.</param>
        protected abstract Task HandleMessageAsync(TPayload message);

        /// <summary>
        /// Handles raw string messages if deserialization fails.
        /// </summary>
        /// <param name="message">Raw JSON string message.</param>
        protected abstract Task HandleStringMessageAsync(string message);


        /// <inheritdoc />
        /// <summary>
        /// Starts the background service and establishes a RabbitMQ connection.
        /// Retries connection on failure and binds the queue to the exchange.
        /// </summary>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Consumer} is starting...", typeof(TConsumer).Name);
            var factory = new ConnectionFactory() { HostName = _rabbitHost };

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(cancellationToken);
                    Channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                    break;
                }
                catch (BrokerUnreachableException ex)
                {
                    _logger.LogWarning("RabbitMQ unreachable: {Error}. Retrying...", ex.Message);
                    await Task.Delay(5000, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unexpected connection error: {Error}", ex.Message);
                    await Task.Delay(5000, cancellationToken);
                }
            }

            if (Channel is null)
            {
                _logger.LogError("Channel setup failed.");
                return;
            }

            //await BindQueueAsync(cancellationToken);

            await BindMultiQueueAsync(GetBindings(), cancellationToken);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (Channel == null)
            {
                _logger.LogError("RabbitMQ channel not initialized.");
                throw new InvalidOperationException("RabbitMQ channel is null.");
            }

            Consumer = new AsyncEventingBasicConsumer(Channel);
            Consumer.ReceivedAsync += async (_, args) =>
            {
                var body = args.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                try
                {
                    var envelope = JsonSerializer.Deserialize<MessageEnvelope<TPayload>>(json);
                    if (envelope!.Payload == null)
                    {
                        _logger.LogWarning("Failed to deserialize message.");
                    }
                    else
                    {
                        await HandleMessageAsync(envelope.Payload);
                    }
                }
                catch (JsonException)
                {
                    await HandleStringMessageAsync(json);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while handling message.");
                }
            };

            await ConsumeMultiAsync(GetBindings(), cancellationToken);
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Consumer} is stopping...", typeof(TConsumer).Name);
            if (Channel is not null)
            {
                await Channel.CloseAsync(cancellationToken);
                _logger.LogInformation("Channel closed.");
            }

            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ connection disposed.");
            await base.StopAsync(cancellationToken);
        }
 
        protected async Task BindMultiQueueAsync(
                List<ExchangeBinding> bindings,
                CancellationToken cancellationToken)
        {
            if (Channel is null)
                throw new InvalidOperationException("RabbitMQ channel is null during queue binding.");

            foreach (var binding in bindings)
            {
                await Channel.QueueDeclareAsync(binding.QueueName, durable: false, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await Channel.QueueBindAsync(binding.QueueName, binding.ExchangeName, binding.RoutingKey ?? "", cancellationToken: cancellationToken);
                _logger.LogInformation("Queue '{Queue}' bound to exchange '{Exchange}' with routing key '{Key}'", binding.QueueName, binding.ExchangeName, binding.RoutingKey);
            }
        }

        protected async Task ConsumeMultiAsync(List<ExchangeBinding> bindings, CancellationToken cancellationToken)
        {
            if (Channel is null || Consumer is null)
                throw new InvalidOperationException("RabbitMQ channel or consumer is null during queue binding.");

            foreach (var binding in bindings)
            {
                await Channel.BasicConsumeAsync(queue: binding.QueueName, autoAck: true, consumer: Consumer, cancellationToken: cancellationToken);
                _logger.LogInformation("New and improved virtual {Consumer} is now consuming messages on '{Queue}'", typeof(TConsumer).Name, binding.QueueName);
            }
        }

    }
}


