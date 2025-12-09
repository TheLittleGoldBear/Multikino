namespace Multikino.Models.View
{
    public class PurchaseTicketViewModel
    {
        public int ScreeningId { get; set; }
        public string MovieTitle { get; set; } = null!;
        public string HallName { get; set; } = null!;
        public System.DateTime StartTime { get; set; }
        public decimal Price { get; set; }
    }
}