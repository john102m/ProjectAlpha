//booking service

using BookingService.Services;
using Serilog;
using Shared.Contracts.Common;

var builder = WebApplication.CreateBuilder(args);
LoggingConfigurator.Configure();

builder.Host.UseSerilog();  // Replace default logger with Serilog
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddScoped<IBookingRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<BookingRepository>>();
    var connectionString = config.GetConnectionString("BookingDatabase")!;
    return new BookingRepository(connectionString, logger);
});
builder.Services.AddControllers();

var app = builder.Build();

//Declare RMQ exchange here to ensure it exists BEFORE consumers bind
using (var scope = app.Services.CreateScope())
{
    var rabbit = scope.ServiceProvider.GetRequiredService<IMessageService>();
    await rabbit.SetupAsync();
}

app.MapControllers();

app.Run();
