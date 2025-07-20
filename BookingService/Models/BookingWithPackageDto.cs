namespace BookingService.Models
{
    public class BookingWithPackageDto
    {
        public int Id { get; set; }
        public string GuestName { get; set; } = default!;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public int PackageId { get; set; }

        // Enriched from catalog.packages
        public string PackageName { get; set; } = default!;
        public string PackageDescription { get; set; } = default!;
        public decimal PackageBasePrice { get; set; }
    }

}
