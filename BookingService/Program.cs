//booking service

using BookingService.Extensions;
using BookingService.Services;
using Serilog;
using Shared.Contracts.Common;
using Shared.Contracts.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);
LoggingConfigurator.Configure();

builder.Host.UseSerilog();  // Replace default logger with Serilog
builder.Services.AddMessaging<BookingMessagePublisher>();
builder.Services.AddBookingRepository(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
