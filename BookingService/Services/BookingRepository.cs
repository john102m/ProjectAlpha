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

        public async Task<IEnumerable<Reservation>> GetReservationsAsync()
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "SELECT id, guestname, checkin, checkout, package, packageid, totalprice FROM booking.reservations";
            return await db.QueryAsync<Reservation>(sql);
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "SELECT id, guestname, checkin, checkout, package, packageid, totalprice FROM booking.reservations WHERE id = @Id";
            return await db.QuerySingleOrDefaultAsync<Reservation>(sql, new { Id = id });
        }

        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"
                INSERT INTO booking.reservations (guestname, checkin, checkout, package, packageid, totalprice)
                VALUES (@GuestName, @CheckIn, @CheckOut, @Package, @PackageId, @TotalPrice)
                RETURNING id;";
            var id = await db.ExecuteScalarAsync<int>(sql, reservation);
            reservation.Id = id;
            return reservation;
        }

        public async Task<bool> UpdateReservationAsync(int id, Reservation reservation)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"
                UPDATE booking.reservations
                SET guestname = @GuestName,
                    checkin = @CheckIn,
                    checkout = @CheckOut,
                    package = @Package,
                    packageid = @PackageId,
                    totalprice = @TotalPrice
                WHERE id = @Id;";
            var rows = await db.ExecuteAsync(sql, new { reservation.GuestName, reservation.CheckIn, reservation.CheckOut, reservation.Package, reservation.PackageId, reservation.TotalPrice, Id = id });
            return rows > 0;
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "DELETE FROM booking.reservations WHERE id = @Id";
            var rows = await db.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<IEnumerable<BookingWithPackageDto>> GetEnrichedReservationsAsync()
        {
            const string sql = @"
        SELECT 
            r.id,
            r.guestname,
            r.checkin,
            r.checkout,
            r.totalprice,
            r.packageid,
            p.name AS packageName,
            p.description AS packageDescription,
            p.price AS packageBasePrice
        FROM booking.reservations r
        JOIN catalog.packages p ON r.packageid = p.id;";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<BookingWithPackageDto>(sql);
        }

        public async Task<IEnumerable<BookingWithPackageDto>> SearchReservationsAsync(string guestName)
        {
            const string sql = @"
        SELECT 
            r.id,
            r.guestname,
            r.checkin,
            r.checkout,
            r.totalprice,
            r.packageid,
            p.name AS packageName,
            p.description AS packageDescription,
            p.price AS packageBasePrice
        FROM booking.reservations r
        JOIN catalog.packages p ON r.packageid = p.id
        WHERE r.guestname ILIKE @Query";

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<BookingWithPackageDto>(sql, new { Query = $"%{guestName}%" });
        }


    }
}
