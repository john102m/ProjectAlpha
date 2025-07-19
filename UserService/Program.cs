using Serilog;
using Shared.Contracts.Common;


var builder = WebApplication.CreateBuilder(args);
LoggingConfigurator.Configure();

builder.Host.UseSerilog();  // Replace default logger with Serilog
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();
