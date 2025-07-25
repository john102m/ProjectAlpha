//User Service

using Serilog;
using Shared.Contracts.Common;
using UserService.Services;
using Shared.Messaging.Infrastructure.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

LoggingConfigurator.Configure();                            
builder.Host.UseSerilog();

builder.AddMessagingPublisherServices<UserMessagePublisher>();
builder.Services.AddControllers();                             

var app = builder.Build();
app.MapControllers();
app.Run();
