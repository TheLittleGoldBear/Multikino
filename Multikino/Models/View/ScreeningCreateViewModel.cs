using Microsoft.AspNetCore.Mvc.Rendering;

namespace Multikino.Models.View
{
    public class ScreeningCreateViewModel
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now.AddHours(1);
        public string Language { get; set; } = "PL";
        public bool Is3D { get; set; }

        public IEnumerable<SelectListItem>? MovieOptions { get; set; }
        public IEnumerable<SelectListItem>? HallOptions { get; set; }
    }
}
