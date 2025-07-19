//api gateway

using Serilog;
using Shared.Contracts.Common;

var builder = WebApplication.CreateBuilder(args);

LoggingConfigurator.Configure();
builder.Host.UseSerilog();  // Replace default logger with Serilog
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();


// Add YARP configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
app.MapControllers();
app.MapReverseProxy();
app.Run();
