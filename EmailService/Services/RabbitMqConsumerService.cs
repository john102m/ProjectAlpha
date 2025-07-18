using EmailService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace EmailService.Services
{
    public class RabbitMqConsumerService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

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
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error: {ex.Message}");
                }

                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: "booking-confirmed",
                autoAck: true,
                consumer: consumer
            );

            // Keep the background service alive
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

}
