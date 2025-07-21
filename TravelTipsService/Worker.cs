using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Shared.Contracts.MessagingModels;
using System.Text;
using System.Text.Json;

namespace TravelTipsService;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private IConnection? _connection = null;
    private IChannel? _channel = null;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Consumer Service is starting...");
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var factory = new ConnectionFactory() { HostName = rabbitHost };
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                break; // Success — break out of retry loop
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogWarning($"RabbitMQ unreachable: {ex.Message}. Retrying in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error while connecting: {ex.Message}. Retrying in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
        if (_channel is null)
        {
            _logger.LogError("Failed to connect to RabbitMQ channel.");
            return;
        }


        //await _channel.ExchangeDeclareAsync("booking-events", ExchangeType.Fanout);
        await _channel.QueueDeclareAsync("booking-tips", false, false, false, cancellationToken: cancellationToken);
        await _channel.QueueBindAsync("booking-tips", "booking-events", "", cancellationToken: cancellationToken);
        await _channel.QueueBindAsync("booking-tips", "booking-direct", "travel-tips", cancellationToken: cancellationToken);

        await base.StartAsync(cancellationToken);

    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("🔒 Channel closed asynchronously.");
        }

        _connection?.Dispose();  // Still safe to dispose connection
        _logger.LogInformation("🛑 RabbitMQ connection disposed.");

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (_channel == null)
        {
            _logger.LogError("RabbitMQ channel not initialized before ExecuteAsync.");
            throw new InvalidOperationException("RabbitMQ channel is null.");
        }

        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var jsonString = Encoding.UTF8.GetString(body);

                try
                {
                    var jsonDoc = JsonDocument.Parse(jsonString);
                    var booking = JsonSerializer.Deserialize<BookingMessage>(jsonDoc);
                    _logger.LogInformation($"Received booking #{booking?.BookingId} for {booking?.Username} → {booking?.Metadata} | {booking?.PackageRef}");
                }
                catch (JsonException)
                {

                    _logger.LogInformation($"Received secret message: {jsonString}");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
            }
            await Task.CompletedTask;
        };

        await _channel!.BasicConsumeAsync("booking-tips", true, consumer);
        _logger.LogInformation("Worker is now listening for booking-tips messages.");

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
        }
    }

}
