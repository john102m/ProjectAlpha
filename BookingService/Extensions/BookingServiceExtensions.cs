using BookingService.Services;

namespace BookingService.Extensions
{
    public static class BookingServiceExtensions
    {
        public static IServiceCollection AddBookingRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBookingRepository>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<BookingRepository>>();
                var connectionString = configuration.GetConnectionString("BookingDatabase")!;
                return new BookingRepository(connectionString, logger);
            });

            return services;
        }
    }
}



