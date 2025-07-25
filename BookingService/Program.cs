//Booking Service

using BookingService.Extensions;
using BookingService.Services;
using Serilog;
using Shared.Contracts.Common;
using Shared.Messaging.Infrastructure.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

//look in GlobalMessaging.props for this json - for dev have switched to xml
//builder.Configuration.AddJsonFile("global-messaging.json", optional: false, reloadOnChange: true);

LoggingConfigurator.Configure();
builder.Host.UseSerilog();  // Replace default logger with Serilog

builder.AddMessagingPublisherServices<BookingMessagePublisher>();
builder.Services.AddBookingRepository(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
