using CatalogService.Models;
using CatalogService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("products")]
    public class CatalogController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICatalogRepository _repo;

        public CatalogController(IConfiguration configuration, ICatalogRepository repo)
        {
            _configuration = configuration;
            _repo = repo;
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
            var products = await _repo.GetProductsAsync();
            return Ok(products);
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
                return Problem("Failed to create product", statusCode: 500);
            }
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        // 🟠 PUT (update existing product)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var success = await _repo.UpdateProductAsync(id, updatedProduct);
            return success ? Ok(updatedProduct) : NotFound();
        }

        // 🔴 DELETE product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _repo.DeleteProductAsync(id);
            return success ? Ok("Product removed") : NotFound();
        }
    }
}
