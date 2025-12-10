namespace Multikino.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int DurationMin { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal BasePrice { get; set; }
        public byte[]? PosterData { get; set; }
        public string? PosterContentType { get; set; }
        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}
