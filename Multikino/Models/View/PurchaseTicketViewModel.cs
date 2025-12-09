using System.ComponentModel.DataAnnotations;

namespace Multikino.Models.View
{
    public class PurchaseTicketViewModel
    {
        [Required]
        public int ScreeningId { get; set; }

        [Required(ErrorMessage = "Tytuł filmu jest wymagany")]
        public string MovieTitle { get; set; } = null!;

        [Required(ErrorMessage = "Nazwa sali jest wymagana")]
        public string HallName { get; set; } = null!;

        [Required(ErrorMessage = "Data seansu jest wymagana")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(0, double.MaxValue, ErrorMessage = "Cena musi być większa lub równa 0")]
        public decimal Price { get; set; }
    }
}