namespace Multikino.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;
        public string? Email { get; set; }

        public string PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;

        public string Role { get; set; } = "Client";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
