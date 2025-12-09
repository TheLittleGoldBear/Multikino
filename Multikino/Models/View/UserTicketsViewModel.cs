namespace Multikino.Models.View
{
    public class UserTicketsViewModel
    {
        public IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
