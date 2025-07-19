using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Shared.Contracts.MessagingModels;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace EmailService.Services
{
    public class RabbitMqConsumerService(ILogger<RabbitMqConsumerService> logger) : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumerService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service is starting...");
            var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var factory = new ConnectionFactory() { HostName = rabbitHost };

            IConnection? connection = null;
            IChannel? channel = null;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    connection = await factory.CreateConnectionAsync();
                    channel = await connection.CreateChannelAsync();
                    break; // Success — break out of retry loop
                }
                catch (BrokerUnreachableException ex)
                {
                    _logger.LogWarning($"RabbitMQ unreachable: {ex.Message}. Retrying in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error while connecting: {ex.Message}. Retrying in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            if (channel is null)
            {
                _logger.LogError("Failed to connect to RabbitMQ channel.");
                return;
            }

            await channel.QueueDeclareAsync(
                queue: "booking-confirmed",
                durable: false,
                exclusive: false,
                autoDelete: false
            );

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    var body = args.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var booking = JsonSerializer.Deserialize<BookingMessage>(json);

                    Debug.WriteLine($"📩 Received booking #{booking?.BookingId} for {booking?.UserEmail} → {booking?.RoomType}");
                    _logger.LogInformation($"📩 Received booking #{booking?.BookingId} for {booking?.UserEmail} → {booking?.RoomType}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error: {ex.Message}");
                    _logger.LogError(ex, ex.Message);
                }

                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: "booking-confirmed",
                autoAck: true,
                consumer: consumer
            );

            // Keep the service alive
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

    }

}
