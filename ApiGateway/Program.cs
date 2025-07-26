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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // React dev server default
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();


// Add YARP configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("AllowReactApp");
app.MapControllers();
app.MapReverseProxy();
app.Run();
