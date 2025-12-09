using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Multikino.Models;
using Multikino.Models.View;
using Multikino.Services;
using System.Security.Claims;


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

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            List<Ticket> myTickets = new List<Ticket>();
            if (int.TryParse(userIdClaim, out var parsed))
            {
                userId = parsed;
                myTickets = (List<Ticket>)await _ticketService.GetTicketsForUserAsync(userId.Value);
            }

            var vm = new ScreeningListViewModel
            {
                Screenings = screenings,
                MyTickets = myTickets,
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


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(PurchaseTicketViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // userId
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Challenge();

            // pobieramy seans
            var screening = await _ticketService.GetScreeningAsync(vm.ScreeningId);
            if (screening == null)
            {
                ModelState.AddModelError("", "Nie znaleziono seansu.");
                return View(vm);
            }

            var sold = screening.Tickets?.Count() ?? 0;
            var free = screening.Hall.Capacity - sold;

            if (free <= 0)
            {
                ModelState.AddModelError("", "Brak wolnych miejsc na ten seans.");
                return View(vm);
            }

            var result = await _ticketService.PurchaseTicketAsync(vm.ScreeningId, userId);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Nie udało się kupić biletu.");
                return View(vm);
            }

            return RedirectToAction("Index");
        }

    }
}