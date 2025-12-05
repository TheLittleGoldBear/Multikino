namespace Multikino.Models
{
    public class Screening
    {
        public int Id { get; set; }
        public int HallId { get; set; }
        public int MovieId { get; set; }
        public DateTime StartTime { get; set; }
        public string Language { get; set; } = "PL";
        public bool Is3D { get; set; }

        public Hall Hall { get; set; } = null!;
        public Movie Movie { get; set; } = null!;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
