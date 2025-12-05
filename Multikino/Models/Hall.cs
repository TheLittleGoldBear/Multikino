namespace Multikino.Models
{
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public bool Is3D { get; set; }

        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}
