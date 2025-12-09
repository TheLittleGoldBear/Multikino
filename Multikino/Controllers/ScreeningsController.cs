using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Multikino.Models.View;
using Multikino.Models;
using Multikino.Services;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Multikino.Controllers
{
    public class ScreeningsController : Controller
    {
        private readonly ITicketService _ticketService;


        public ScreeningsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? sortOrder)
        {
            // Przygotuj toggle'y sortowania (kliknięcie zmienia sposób sortowania)
            ViewBag.DateSort = sortOrder == "date" ? "date_desc" : "date";
            ViewBag.MovieSort = sortOrder == "movie" ? "movie_desc" : "movie";
            ViewBag.HallSort = sortOrder == "hall" ? "hall_desc" : "hall";

            var screenings = await _ticketService.GetUpcomingScreeningsAsync(search, sortOrder);

            var vm = new ScreeningListViewModel
            {
                Screenings = screenings,
                Search = search,
                SortOrder = sortOrder
            };

            return View(vm);
        }


        // GET: /Screenings/Details/5 -> pokazuje szczegóły i formularz kupna
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var s = await _ticketService.GetScreeningAsync(id);
            if (s == null) return NotFound();


            decimal price = s.Movie.BasePrice + ((s.Is3D || s.Hall.Is3D) ? 5m : 0m);


            var vm = new PurchaseTicketViewModel
            {
                ScreeningId = s.Id,
                MovieTitle = s.Movie.Title,
                HallName = s.Hall.Name,
                StartTime = s.StartTime,
                Price = price
            };


            return View(vm);
        }


        // POST: /Screenings/Details/5 -> kupno
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(PurchaseTicketViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);


            // pobierz userId z claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out var parsed)) userId = parsed;


            var result = await _ticketService.PurchaseTicketAsync(vm.ScreeningId, userId);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Nieznany błąd podczas zakupu.");
                return View(vm);
            }


            return RedirectToAction("MyTickets", "Tickets");
        }
    }
}