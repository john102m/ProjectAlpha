
using EmailService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHostedService<RabbitMqConsumerService>(); 

var app = builder.Build();
app.MapControllers();

app.Run();
