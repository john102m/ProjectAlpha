using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Exceptions;
using Shared.Messaging.Infrastructure.Models;
using Shared.Messaging.Infrastructure.MessageModels;

namespace Shared.Messaging.Infrastructure.RabbitMq
{
    /// <summary>
    /// Base class for RabbitMQ message publishers.
    /// Manages connection lifecycle and provides helper methods for exchange declaration and publishing.
    /// </summary>
    /// <typeparam name="TPublisher">The concrete publisher class type.</typeparam>
    public abstract class RabbitMqPublisherBase<TPublisher>(ILogger<TPublisher> logger) : IDisposable
    {
        private readonly ILogger<TPublisher> _logger = logger;
        private readonly string _rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        private IConnection? _connection;
        protected IChannel? _channel;

        protected string ToRabbitMqType(MessagingExchangeType type)
        {
            return type switch
            {
                MessagingExchangeType.Fanout => ExchangeType.Fanout,
                MessagingExchangeType.Direct => ExchangeType.Direct,
                MessagingExchangeType.Topic => ExchangeType.Topic,
                MessagingExchangeType.Headers => ExchangeType.Headers,
                _ => throw new ArgumentOutOfRangeException(nameof(type), "Unsupported exchange type.")
            };
        }

        /// <summary>
        /// Initializes RabbitMQ connection and declares specified exchanges.
        /// Retries connection on failure and logs each attempt.
        /// </summary>
        /// <param name="exchanges">Tuple array of exchange definitions.</param>
        /// <param name="cancellationToken">Token to support graceful cancellation.</param>
        public async Task InitializeAsync(
            (string ExchangeName, string ExchangeType, bool Durable)[] exchanges,
            CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory { HostName = _rabbitHost };

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync();
                    _channel = await _connection.CreateChannelAsync();
                    _logger.LogInformation("RabbitMQ publisher connection established.");
                    break;
                }
                catch (BrokerUnreachableException ex)
                {
                    _logger.LogWarning("Publisher could not reach RabbitMQ: {Error}. Retrying...", ex.Message);
                    await Task.Delay(3000, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unexpected publisher connection error: {Error}", ex.Message);
                    await Task.Delay(3000, cancellationToken);
                }
            }

            if (_channel is null)
            {
                _logger.LogError("Publisher channel setup failed.");
                throw new InvalidOperationException("RabbitMQ publisher channel not initialized.");
            }

            foreach (var (name, type, durable) in exchanges)
            {
                await _channel.ExchangeDeclareAsync(name, type, durable, cancellationToken: cancellationToken);
                _logger.LogInformation("Exchange declared: {Exchange}", name);
            }
        }

        /// <summary>
        /// Publishes a message object to the specified exchange with the given routing key.
        /// </summary>
        /// <param name="exchange">Target exchange.</param>
        /// <param name="routingKey">Message routing key.</param>
        /// <param name="payload">Payload object to serialize and publish.</param>
        public async Task PublishAsync<T>(string exchangeName, string routingKey, T message)
        {
            var envelope = new MessageEnvelope<T>
            {
                Payload = message,
                SourceService = typeof(TPublisher).Name,
                TraceId = Guid.NewGuid().ToString()
                // add other metadata here
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope));

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                MessageId = envelope.MessageId.ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                //Headers = new Dictionary<string, object>
                //{
                //    ["TraceId"] = envelope.TraceId,
                //    ["SourceService"] = envelope.SourceService
                //}
            };

            await _channel!.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey,
                body: body.AsMemory(),            // Make sure it's ReadOnlyMemory<byte>
                mandatory: false,                 // Can be true if you care about routing failures
                basicProperties: properties,
                cancellationToken: CancellationToken.None // Or use a real token if available
            );
        }

        /// <summary>
        /// Disposes RabbitMQ resources, including channel and connection.
        /// </summary>
        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
