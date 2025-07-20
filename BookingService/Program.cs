//booking service

using BookingService.Services;
using Serilog;
using Shared.Contracts.Common;

var builder = WebApplication.CreateBuilder(args);
LoggingConfigurator.Configure();

builder.Host.UseSerilog();  // Replace default logger with Serilog
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IBookingRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<BookingRepository>>();
    var connectionString = config.GetConnectionString("BookingDatabase")!;
    return new BookingRepository(connectionString, logger);
});
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();
