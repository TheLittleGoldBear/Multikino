namespace Multikino.Models.View
{
    public class RevenueByMovieReportViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IEnumerable<RevenueByMovieItem> Items { get; set; } = Enumerable.Empty<RevenueByMovieItem>();
    }
}
