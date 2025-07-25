//email service

using EmailService.Services;
using Serilog;
using Shared.Contracts.Common;
using Shared.Messaging.Infrastructure.ServiceExtensions;
var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddJsonFile("global-messaging.json", optional: false, reloadOnChange: true);

LoggingConfigurator.Configure();

builder.Host.UseSerilog();  // Replace default logger with Serilog

builder.Services.AddControllers();
builder.AddMessagingConsumerServices<EmailConsumer>();

var app = builder.Build();
app.MapControllers();

app.Run();
