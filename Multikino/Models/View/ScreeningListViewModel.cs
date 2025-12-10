namespace Multikino.Models.View
{
    public class ScreeningListViewModel
    {
        public IEnumerable<Screening> Screenings { get; set; } = new List<Screening>();
        public IEnumerable<Ticket> MyTickets { get; set; } = new List<Ticket>();
        public string? Search { get; set; }
        public string? SortOrder { get; set; }
        public string? TicketSearch { get; set; }
        public string? TicketSort { get; set; }
    }
}