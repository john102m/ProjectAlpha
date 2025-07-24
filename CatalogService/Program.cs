using CatalogService.Services;
using Serilog;
using Shared.Contracts.Common;

var builder = WebApplication.CreateBuilder(args);
LoggingConfigurator.Configure();

builder.Host.UseSerilog();  // Replace default logger with Serilog

builder.Services.AddControllers();

builder.Services.AddScoped<ICatalogRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<CatalogRepository>>();
    var connectionString = config.GetConnectionString("CatalogDatabase")!;
    return new CatalogRepository(connectionString, logger);
});



var app = builder.Build();
app.MapControllers();

app.Run();
