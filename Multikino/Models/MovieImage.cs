namespace Multikino.Models
{
    public class MovieImage
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public byte[] Data { get; set; } = null!;
        public DateTime UploadedAt { get; set; }

        public Movie Movie { get; set; } = null!;
    }
}
