using Microsoft.AspNetCore.Mvc;
using Multikino.Services;

namespace Multikino.Controllers
{
    public class CinemaController : Controller
    {
        private readonly ICinemaService _cinemaService;

        public CinemaController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        // /Cinema/Movies
        public async Task<IActionResult> Movies(string? search, string? sortOrder)
        {
            var movies = await _cinemaService.GetMoviesAsync(search, sortOrder);
            return View(movies);
        }

        // /Cinema/Screenings?movieId=1&from=2025-01-01&to=2025-01-31
        public async Task<IActionResult> Screenings(int movieId, DateTime? from, DateTime? to)
        {
            var screenings = await _cinemaService.GetScreeningsAsync(movieId, from, to);
            ViewBag.MovieId = movieId;
            return View(screenings);
        }

        // GET: /Cinema/Reserve/123
        public IActionResult Reserve(int screeningId)
        {
            ViewBag.ScreeningId = screeningId;
            return View();
        }

        // POST: /Cinema/Reserve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int screeningId, string seatNumber)
        {
            // tu później możesz wziąć userId z zalogowanego użytkownika
            bool ok = await _cinemaService.ReserveTicketAsync(screeningId, seatNumber, null);

            if (!ok)
            {
                ModelState.AddModelError("", "To miejsce jest już zajęte lub seans nie istnieje.");
                ViewBag.ScreeningId = screeningId;
                return View();
            }

            // proste potwierdzenie
            return RedirectToAction("ReservationSuccess");
        }

        public IActionResult ReservationSuccess()
        {
            return View();
        }
    }
}
