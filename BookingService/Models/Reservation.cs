namespace BookingService.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string GuestName { get; set; } = default!;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Package { get; set; } = default!;
        public decimal TotalPrice { get; set; }
        public int PackageId { get; set; }  
    }
}

