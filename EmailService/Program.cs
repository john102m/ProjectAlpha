//email service

using EmailService.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var customThemeStyles =
    new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
    {
        {
            ConsoleThemeStyle.Text, new SystemConsoleThemeStyle
            {
                Foreground = ConsoleColor.Green,
            }
        },
        {
            ConsoleThemeStyle.String, new SystemConsoleThemeStyle
            {
                Foreground = ConsoleColor.Yellow,
            }
        },
    };

var customTheme = new SystemConsoleTheme(customThemeStyles);



var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    //.Enrich.FromLogContext()
    .WriteTo.Console(theme: customTheme, outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")   // This outputs to Docker console
    .CreateLogger();

builder.Host.UseSerilog();  // Replace default logger with Serilog

builder.Services.AddControllers();
builder.Services.AddHostedService<RabbitMqConsumerService>(); 

var app = builder.Build();
app.MapControllers();

app.Run();
