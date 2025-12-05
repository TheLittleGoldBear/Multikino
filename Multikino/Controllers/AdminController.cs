using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Multikino.Models;
using Multikino.Models.View;
using Multikino.Services;

namespace Multikino.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index(
            string? movieSearch,
            string? movieSortOrder,
            string? hallSearch,
            string? hallSortOrder,
            string? screeningSearch,
            string? screeningSortOrder)
        {
            // sortowanie nagłówków filmów
            ViewBag.MovieTitleSort = string.IsNullOrEmpty(movieSortOrder) ? "title_desc" : "";
            ViewBag.MoviePriceSort = movieSortOrder == "price" ? "price_desc" : "price";
            ViewBag.MovieReleaseSort = movieSortOrder == "release" ? "release_desc" : "release";

            // sortowanie nagłówków sal
            ViewBag.HallNameSort = string.IsNullOrEmpty(hallSortOrder) ? "name_desc" : "";
            ViewBag.HallCapacitySort = hallSortOrder == "capacity" ? "capacity_desc" : "capacity";

            // sortowanie nagłówków seansów
            ViewBag.ScreeningDateSort = string.IsNullOrEmpty(screeningSortOrder) ? "date_desc" : "";
            ViewBag.ScreeningMovieSort = screeningSortOrder == "movie" ? "movie_desc" : "movie";
            ViewBag.ScreeningHallSort = screeningSortOrder == "hall" ? "hall_desc" : "hall";

            var vm = new AdminDashboardViewModel
            {
                Movies = await _adminService.GetMoviesAsync(movieSearch, movieSortOrder),
                Halls = await _adminService.GetHallsAsync(hallSearch, hallSortOrder),
                Screenings = await _adminService.GetScreeningsAsync(screeningSearch, screeningSortOrder),

                MovieSearch = movieSearch,
                MovieSortOrder = movieSortOrder,
                HallSearch = hallSearch,
                HallSortOrder = hallSortOrder,
                ScreeningSearch = screeningSearch,
                ScreeningSortOrder = screeningSortOrder
            };

            return View(vm); // to trafi do Views/Admin/Index.cshtml
        }

        // GET: /Admin/MovieDetails/5
        public async Task<IActionResult> MovieDetails(int id)
        {
            var movie = await _adminService.GetMovieAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // GET: /Admin/EditMovie/5
        public async Task<IActionResult> EditMovie(int id)
        {
            var movie = await _adminService.GetMovieAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // POST: /Admin/EditMovie/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(int id, Movie movie)
        {
            if (id != movie.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(movie);

            var ok = await _adminService.UpdateMovieAsync(movie);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _adminService.GetMovieAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost, ActionName("DeleteMovie")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovieConfirmed(int id)
        {
            await _adminService.DeleteMovieAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/CreateMovie
        public IActionResult CreateMovie()
        {
            var model = new Movie
            {
                ReleaseDate = DateTime.Today,
                DurationMin = 90,
                BasePrice = 25m
            };
            return View(model);
        }

        // POST: /Admin/CreateMovie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMovie(Movie movie)
        {
            if (!ModelState.IsValid)
                return View(movie);

            await _adminService.CreateMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }

        // ================== SEANSE ==================

        // GET: /Admin/EditScreening/5
        public async Task<IActionResult> EditScreening(int id)
        {
            var screening = await _adminService.GetScreeningAsync(id);
            if (screening == null) return NotFound();

            var movies = await _adminService.GetMoviesAsync();
            var halls = await _adminService.GetHallsAsync();

            var vm = new ScreeningCreateViewModel
            {
                Id = screening.Id,
                MovieId = screening.MovieId,
                HallId = screening.HallId,
                StartTime = screening.StartTime,
                Language = screening.Language,
                Is3D = screening.Is3D,
                MovieOptions = movies.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Title
                }),
                HallOptions = halls.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"{h.Name} (miejsc: {h.Capacity})"
                })
            };

            return View(vm);
        }

        // POST: /Admin/EditScreening/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditScreening(int id, ScreeningCreateViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await FillDropdowns(vm);
                return View(vm);
            }

            vm.StartTime = new DateTime(
                vm.StartTime.Year,
                vm.StartTime.Month,
                vm.StartTime.Day,
                vm.StartTime.Hour,
                vm.StartTime.Minute,
                0
            );

            var screening = new Screening
            {
                Id = vm.Id,
                MovieId = vm.MovieId,
                HallId = vm.HallId,
                StartTime = vm.StartTime,
                Language = vm.Language,
                Is3D = vm.Is3D
            };

            var ok = await _adminService.UpdateScreeningAsync(screening);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/CreateScreening
        public async Task<IActionResult> CreateScreening()
        {
            var movies = await _adminService.GetMoviesAsync();
            var halls = await _adminService.GetHallsAsync();

            var vm = new ScreeningCreateViewModel
            {
                MovieOptions = movies.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Title
                }),
                HallOptions = halls.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = $"{h.Name} (miejsc: {h.Capacity})"
                }),
                StartTime = DateTime.Now.AddHours(2)
            };

            return View(vm);
        }

        // POST: /Admin/CreateScreening
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateScreening(ScreeningCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await FillDropdowns(vm);
                return View(vm);
            }

            var allScreenings = await _adminService.GetScreeningsAsync();
            bool conflict = allScreenings.Any(s =>
                s.HallId == vm.HallId &&
                s.StartTime == vm.StartTime);

            if (conflict)
            {
                ModelState.AddModelError("", "W wybranej sali jest już seans o tej godzinie.");
                await FillDropdowns(vm);
                return View(vm);
            }

            var screening = new Screening
            {
                MovieId = vm.MovieId,
                HallId = vm.HallId,
                StartTime = vm.StartTime,
                Language = vm.Language,
                Is3D = vm.Is3D
            };

            await _adminService.CreateScreeningAsync(screening);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteScreening(int id)
        {
            var screening = await _adminService.GetScreeningAsync(id);
            if (screening == null) return NotFound();

            return View(screening);
        }

        [HttpPost, ActionName("DeleteScreening")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScreeningConfirmed(int id)
        {
            await _adminService.DeleteScreeningAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task FillDropdowns(ScreeningCreateViewModel vm)
        {
            var movies = await _adminService.GetMoviesAsync();
            var halls = await _adminService.GetHallsAsync();

            vm.MovieOptions = movies.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            });
            vm.HallOptions = halls.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = $"{h.Name} (miejsc: {h.Capacity})"
            });
        }

        // ================== SALE (tylko podgląd) ==================

        // GET: /Admin/Halls
        public async Task<IActionResult> Halls(string? search, string? sortOrder)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CapacitySort = sortOrder == "capacity" ? "capacity_desc" : "capacity";

            var halls = await _adminService.GetHallsAsync(search, sortOrder);
            return View(halls);
        }
    }
}
