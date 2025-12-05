namespace Multikino.Models.View
{
    public class AdminDashboardViewModel
    {
        public List<Movie> Movies { get; set; } = new();
        public List<Hall> Halls { get; set; } = new();
        public List<Screening> Screenings { get; set; } = new();

        public string? MovieSearch { get; set; }
        public string? MovieSortOrder { get; set; }

        public string? ScreeningSearch { get; set; }  
        public string? ScreeningSortOrder { get; set; }

        public string? HallSearch { get; set; }
        public string? HallSortOrder { get; set; }
    }
}
