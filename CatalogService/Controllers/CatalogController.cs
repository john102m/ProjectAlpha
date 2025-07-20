using CatalogService.Models;
using CatalogService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("packages")]
    public class CatalogController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICatalogRepository _repo;
        private readonly IHostEnvironment _env;

        public CatalogController(IConfiguration configuration, ICatalogRepository repo, IHostEnvironment env)
        {
            _configuration = configuration;
            _repo = repo;
            _env = env;
        }
        [HttpGet("health")]
        public IActionResult Get()
        {
            return Ok("📦 CatalogService is alive and responding!");
        }

        // List products endpoint: GET /home/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _repo.GetProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                var detailMessage = _env.IsDevelopment() ? ex.Message : "Error";
                return Problem(
                    detail: detailMessage,
                    title: "Failed to retrieve products",
                    statusCode: 500
                );
            }
        }

        // 🔵 GET single product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _repo.GetProductByIdAsync(id);
            return product is not null ? Ok(product) : NotFound();
        }

        // 🟡 POST (create new product)
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var created = await _repo.CreateProductAsync(product);
            if (created == null)
            {
                return Problem("Failed to create package", statusCode: 500);
            }
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        // 🟠 PUT (update existing product)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var success = await _repo.UpdateProductAsync(id, updatedProduct);
            return success ? Ok(updatedProduct) : NotFound(new
            {
                message = $"Package with ID {id} not found. Cannot update."
            });
        }

        // 🔴 DELETE product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _repo.DeleteProductAsync(id);
            return success ? Ok("Package removed") : NotFound(new
            {
                message = $"Package with ID {id} not found. Cannot delete."
            });
        }
    }
}
