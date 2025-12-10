namespace Multikino.Models.View
{
    public class TicketSalesReportViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IEnumerable<TicketSalesReportItem> Items { get; set; } = Enumerable.Empty<TicketSalesReportItem>();
    }
}
