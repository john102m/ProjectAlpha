using BookingService.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace BookingService.Services
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(string connectionString, ILogger<BookingRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<IEnumerable<ReservationView>> GetReservationsAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT * from booking.v_reservations_with_package_name2";
            return await db.QueryAsync<ReservationView>(sql);
        }

        public async Task<ReservationView?> GetReservationByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT * from booking.v_reservations_with_package_name2 WHERE id = @Id";
            return await db.QuerySingleOrDefaultAsync<ReservationView>(sql, new { Id = id });
        }

        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"
                INSERT INTO booking.reservations2 (guestname, checkin, checkout, package, packageid, totalprice)
                VALUES (@GuestName, @CheckIn, @CheckOut, @ExtraInfo, @PackageId, @TotalPrice)
                RETURNING id;";
            var id = await db.ExecuteScalarAsync<int>(sql, reservation);
            reservation.Id = id;
            return reservation;
        }

        public async Task<bool> UpdateReservationAsync(int id, Reservation reservation)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"
                UPDATE booking.reservations2
                SET guestname = @GuestName,
                    checkin = @CheckIn,
                    checkout = @CheckOut,
                    package = @ExtraInfo,
                    packageid = @PackageId,
                    totalprice = @TotalPrice
                WHERE id = @Id;";
            var rows = await db.ExecuteAsync(sql, new
            {
                reservation.GuestName,
                reservation.CheckIn,
                reservation.CheckOut,
                reservation.ExtraInfo,
                reservation.PackageId,
                reservation.TotalPrice,
                Id = id
            });
            return rows > 0;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "DELETE FROM booking.reservations2 WHERE id = @Id";
            var rows = await db.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<IEnumerable<BookingWithPackageDto>> GetEnrichedReservationsAsync()
        {
            try
            {
                string sql = @"SELECT * FROM booking.v_reservations_with_package_info2";
                using var connection = new NpgsqlConnection(_connectionString);
                return await connection.QueryAsync<BookingWithPackageDto>(sql);

            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "42501") // permission denied
                {
                    throw new UnauthorizedAccessException("Booking user does not have permission to read the view.", ex);
                }
                throw;
            }
        }

        public async Task<BookingWithPackageDto> GetEnrichedReservationsByIdAsync(int id)
        {
            try
            {
                string sql = @"SELECT * FROM booking.v_reservations_with_package_info2 WHERE id = @Id";
                using var connection = new NpgsqlConnection(_connectionString);
                return await connection.QuerySingleAsync<BookingWithPackageDto>(sql, new { Id = id });

            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "42501") // permission denied
                {
                    throw new UnauthorizedAccessException("Booking user does not have permission to read the view.", ex);
                }
                throw;
            }
        }

        public async Task<IEnumerable<BookingWithPackageDto>> SearchReservationsAsync(string searchTerm)
        {
            const string sql = "SELECT * FROM booking.search_reservations2(@Query, @Fallback);";
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<BookingWithPackageDto>(sql, new
            {
                Query = searchTerm,
                Fallback = $"%{searchTerm.Replace(":*", "")}%"
            });
        }
    }
}
