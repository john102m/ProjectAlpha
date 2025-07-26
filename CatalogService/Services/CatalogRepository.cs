using CatalogService.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace CatalogService.Services
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<CatalogRepository> _logger;

        public CatalogRepository(string connectionString, ILogger<CatalogRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            try
            {
                using IDbConnection db = new NpgsqlConnection(_connectionString);
                var sql = "SELECT id, name, destination, description, nights, price, image_url FROM catalog.packages";
                return await db.QueryAsync<Product>(sql);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetProductsAsync: {ex.Message}");
                _logger.LogError(ex, "Error in GetProductsAsync");
                throw;

            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "SELECT id, name, destination, description, nights, price, image_url FROM catalog.packages WHERE id = @Id";
            return await db.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                using IDbConnection db = new NpgsqlConnection(_connectionString);
                var sql = @"
                INSERT INTO catalog.packages (name, description, price)
                VALUES (@Name, @Description, @Price)
                RETURNING id;";
                var id = await db.ExecuteScalarAsync<int>(sql, product);
                product.Id = id;
                return product;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in CreateProductAsync: {ex.Message}");
                _logger.LogError(ex, "Error in CreateProductAsync");
                return null!;
            }

        }

        public async Task<bool> UpdateProductAsync(int id, Product product)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"
            UPDATE catalog.packages
            SET name = @Name,
                description = @Description,
                price = @Price
            WHERE id = @Id;";
            var rows = await db.ExecuteAsync(sql, new { product.Name, product.Description, product.Price, Id = id });

            _logger.LogInformation("UpdateProductAsync: {Rows} updated", rows);

            return rows > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "DELETE FROM catalog.packages WHERE id = @Id";
            var rows = await db.ExecuteAsync(sql, new { Id = id });

            _logger.LogInformation("DeleteProductAsync: {Rows} deleted", rows);

            return rows > 0;
        }
    }
}
