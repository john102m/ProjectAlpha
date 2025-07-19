//booking service

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")   // This outputs to Docker console
    .CreateLogger();

builder.Host.UseSerilog();  // Replace default logger with Serilog

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();
