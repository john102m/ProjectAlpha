namespace BookingService.Models
{
    public class ReservationView
    {
        public int Id { get; set; }
        public string GuestName { get; set; } = default!;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; } = default!;
        public string ExtraInfo { get; set; } = default!;
    }
}
