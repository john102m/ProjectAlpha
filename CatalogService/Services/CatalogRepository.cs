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
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "SELECT id, name, destination, description, nights, price, image_url AS ImageUrl FROM catalog.packages";
            return await db.QueryAsync<Product>(sql);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "SELECT id, name, destination, description, nights, price, image_url AS ImageUrl FROM catalog.packages WHERE id = @Id";
            return await db.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"
                INSERT INTO catalog.packages (name, destination, description, nights, price, image_url)
                VALUES (@Name, @Destination, @Description, @Nights, @Price, @ImageUrl)
                RETURNING id;";
            var id = await db.ExecuteScalarAsync<int>(sql, product);
            product.Id = id;
            return product;
        }

        public async Task<bool> UpdateProductAsync(int id, Product product)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = @"
                UPDATE catalog.packages
                SET name = @Name,
                    destination = @Destination,
                    description = @Description,
                    nights = @Nights,
                    price = @Price,
                    image_url = @ImageUrl
                WHERE id = @Id;
            ";
            var parameters = new
            {
                product.Name,
                product.Destination,
                product.Description,
                product.Nights,
                product.Price,
                product.ImageUrl,
                Id = id
            };

            var rows = await db.ExecuteAsync(sql, parameters);
            return rows > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString);
            var sql = "DELETE FROM catalog.packages WHERE id = @Id";
            var rows = await db.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
