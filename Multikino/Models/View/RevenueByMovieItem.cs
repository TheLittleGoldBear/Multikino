namespace Multikino.Models.View
{
    public class RevenueByMovieItem
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = "";
        public int TicketsSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
