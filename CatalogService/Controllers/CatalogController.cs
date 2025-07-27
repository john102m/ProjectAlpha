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
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IConfiguration configuration, ICatalogRepository repo, IHostEnvironment env, ILogger<CatalogController> logger)
        {
            _configuration = configuration;
            _repo = repo;
            _env = env;
            _logger = logger;
        
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

        // GET single product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _repo.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with id {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // POST (create new product)
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            try
            {
                var createdProduct = await _repo.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // 🟠 PUT (update existing product)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            try
            {
                var updated = await _repo.UpdateProductAsync(id, updatedProduct);
                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with id {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // DELETE product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var deleted = await _repo.DeleteProductAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with id {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
