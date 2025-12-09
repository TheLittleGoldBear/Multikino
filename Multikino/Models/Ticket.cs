namespace Multikino.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ScreeningId { get; set; }
        public int? UserId { get; set; }
        public decimal Price { get; set; }
        public DateTime SoldAt { get; set; }

        public Screening Screening { get; set; } = null!;
        public User? User { get; set; }
    }
}
