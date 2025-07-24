
using Serilog;
using Shared.Contracts.Common;
using Shared.Contracts.MessagingBaseClasses;
using UserService.Services;


var builder = WebApplication.CreateBuilder(args);
LoggingConfigurator.Configure();

builder.Host.UseSerilog();  // Replace default logger with Serilog
builder.Services.AddSingleton<IMessagePublisher, UserMessagePublisher>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();
