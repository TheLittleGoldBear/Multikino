namespace Multikino.Models.View
{
    public class ScreeningListViewModel
    {
        public IEnumerable<Screening> Screenings { get; set; } = new List<Screening>();
        public string? Search { get; set; }
        public string? SortOrder { get; set; }
    }
}