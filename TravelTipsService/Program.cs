using Serilog;
using Shared.Contracts.Common;
using TravelTipsService;

var builder = Host.CreateApplicationBuilder(args);

LoggingConfigurator.Configure();
// Set up Serilog before building the host
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger());
builder.Services.AddHostedService<WorkerConsumer>();

var host = builder.Build();
host.Run();


