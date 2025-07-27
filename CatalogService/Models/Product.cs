namespace CatalogService.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Destination { get; set; }
        public string? Description { get; set; }
        public int Nights { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
