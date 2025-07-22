using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Shared.Contracts.MessagingModels;
using System.Text;
using System.Text.Json;

namespace EmailService.Services;

public class RabbitMqConsumerService(ILogger<RabbitMqConsumerService> logger) : BackgroundService
{
    private readonly ILogger<RabbitMqConsumerService> _logger = logger;
    private IConnection? _connection = null;
    private IChannel? _channel = null;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email Consumer Service starting...");
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var factory = new ConnectionFactory() { HostName = rabbitHost };

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                break; // success!
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogWarning($"RabbitMQ unreachable: {ex.Message}. Retrying in 5s...");
                await Task.Delay(5000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Connection error: {ex.Message}. Retrying...");
                await Task.Delay(5000, cancellationToken);
            }
        }

        if (_channel == null)
        {
            _logger.LogError("🚫 Channel setup failed.");
            return;
        }

        await _channel.QueueDeclareAsync("booking-emails", false, false, false, cancellationToken: cancellationToken);
        await _channel.QueueBindAsync("booking-emails", "booking-events", "", cancellationToken: cancellationToken);

        _logger.LogInformation("✅ Email queue and bindings declared.");
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            _logger.LogInformation("Channel closed.");
        }

        _connection?.Dispose();
        _logger.LogInformation("RabbitMQ connection disposed.");
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_channel == null)
        {
            _logger.LogError("Channel not initialized before execution.");
            throw new InvalidOperationException("RabbitMQ channel is null.");
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var booking = JsonSerializer.Deserialize<BookingMessage>(json);
                _logger.LogInformation($"Booking received #{booking?.BookingId} for {booking?.Username} → {booking?.Metadata} | {booking?.PackageRef}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to deserialize booking: {ex.Message}");
            }

            await Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync("booking-emails", true, consumer, cancellationToken);
        _logger.LogInformation("Email consumer is now listening for booking-emails messages.");

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken); // keep-alive
        }
    }
}
