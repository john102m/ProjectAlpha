using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace Shared.Contracts.MessagingModels
{
    /// <summary>
    /// Base class for RabbitMQ consumers used as hosted background services.
    /// Establishes connection, binds to the queue, and delegates message handling.
    /// </summary>
    /// <typeparam name="TMessage">The expected message type to deserialize.</typeparam>
    /// <typeparam name="TConsumer">The derived consumer class type.</typeparam>
    public abstract class RabbitMqConsumerBase<TMessage, TConsumer>(
        ILogger<TConsumer> logger,
        string queueName,
        string exchangeName) : BackgroundService
    {
        private readonly ILogger<TConsumer> _logger = logger;
        private readonly string _queueName = queueName;
        private readonly string _exchangeName = exchangeName;

        /// <summary>
        /// Active RabbitMQ connection instance.
        /// </summary>
        private IConnection? _connection = null;

        /// <summary>
        /// Communication channel with RabbitMQ, initialized after connection.
        /// </summary>
        protected IChannel? Channel { get; private set; }

        /// <summary>
        /// Event-based consumer listening for incoming RabbitMQ messages.
        /// </summary>
        protected AsyncEventingBasicConsumer? Consumer { get; private set; }


        /// <inheritdoc />
        /// <summary>
        /// Starts the background service and establishes a RabbitMQ connection.
        /// Retries connection on failure and binds the queue to the exchange.
        /// </summary>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Consumer} is starting...", typeof(TConsumer).Name);
            var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var factory = new ConnectionFactory() { HostName = rabbitHost };

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

            await BindQueueAsync(cancellationToken);

            await base.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        /// <summary>
        /// Executes the message consumption loop.
        /// Deserializes messages and delegates processing to the derived implementation.
        /// </summary>
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
                    var message = JsonSerializer.Deserialize<TMessage>(json);
                    if (message == null)
                    {
                        _logger.LogWarning("Failed to deserialize message.");
                    }
                    else
                    {
                        await HandleMessageAsync(message);
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

            await ConsumeAsync(cancellationToken);

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// Gracefully shuts down the RabbitMQ consumer and disposes resources.
        /// </summary>
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
        /// <summary>
        /// Handles deserialized messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <param name="message">The strongly typed message object.</param>
        protected abstract Task HandleMessageAsync(TMessage message);

        /// <summary>
        /// Handles raw string messages if deserialization fails.
        /// </summary>
        /// <param name="message">Raw JSON string message.</param>
        protected abstract Task HandleStringMessageAsync(string message);

        /// <summary>
        /// Binds the queue to the exchange using a fanout routing key.
        /// </summary>
        /// <param name="cancellationToken">Token for cancellation support.</param>    
        protected virtual async Task BindQueueAsync(CancellationToken cancellationToken)
        {
            if (Channel is null)
                throw new InvalidOperationException("RabbitMQ channel is null during queue binding.");

            await Channel.QueueDeclareAsync(_queueName, durable: false, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
            await Channel.QueueBindAsync(_queueName, _exchangeName, routingKey: "", cancellationToken: cancellationToken);

            _logger.LogInformation("Queue '{Queue}' bound to exchange '{Exchange}'", _queueName, _exchangeName);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Initiates message consumption using the configured consumer.
        /// </summary>
        /// <param name="cancellationToken">Token for cancellation support.</param>    
        protected virtual async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            if (Channel is null || Consumer is null)
                throw new InvalidOperationException("RabbitMQ channel or consumer is null during queue binding.");

            await Channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: Consumer, cancellationToken: cancellationToken);

            _logger.LogInformation("New and improved virtual {Consumer} is now consuming messages on '{Queue}'", typeof(TConsumer).Name, _queueName);
            await Task.CompletedTask;
        }


    }

}


