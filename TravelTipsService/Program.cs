//Traveltips Service

using Serilog;
using Shared.Contracts.Common;
using Shared.Messaging.Infrastructure.ServiceExtensions;
using TravelTipsService;

var builder = Host.CreateApplicationBuilder(args);

//builder.Configuration.AddJsonFile("global-messaging.json", optional: false, reloadOnChange: true);

LoggingConfigurator.Configure();
// Set up Serilog before building the host
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger());

builder.AddMessagingConsumerServices<WorkerConsumer>();

var host = builder.Build();
host.Run();







