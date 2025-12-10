using Microsoft.EntityFrameworkCore;
using Multikino.Data;
using Multikino.Models;

namespace Multikino.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly MultikinoDbContext _context;

        public CinemaService(MultikinoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetMoviesAsync(string? search, string? sortOrder)
        {
            IQueryable<Movie> query = _context.Movies;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.Title.Contains(search));
            }

            query = sortOrder switch
            {
                "title_desc" => query.OrderByDescending(m => m.Title),
                "duration" => query.OrderBy(m => m.DurationMin),
                "duration_desc" => query.OrderByDescending(m => m.DurationMin),
                _ => query.OrderBy(m => m.Title)
            };

            return await query.ToListAsync();
        }

        public async Task<Movie?> GetMovieWithScreeningsAsync(int movieId)
        {
            return await _context.Movies
                .Include(m => m.Screenings)        // powiązane seanse
                    .ThenInclude(s => s.Hall)      // plakaty
                .FirstOrDefaultAsync(m => m.Id == movieId);
        }

        public async Task<List<Screening>> GetScreeningsAsync(int movieId, DateTime? from, DateTime? to)
        {
            IQueryable<Screening> query = _context.Screenings
                .Include(s => s.Hall)
                .Include(s => s.Movie)
                .Where(s => s.MovieId == movieId);

            if (from.HasValue)
                query = query.Where(s => s.StartTime >= from.Value);

            if (to.HasValue)
                query = query.Where(s => s.StartTime <= to.Value);

            return await query
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<bool> ReserveTicketAsync(int screeningId,  int? userId = null)
        {
            // sprawdzamy czy miejsce jest wolne
            bool taken = await _context.Tickets
                .AnyAsync(t => t.ScreeningId == screeningId );

            if (taken)
                return false;

            // pobieramy seans (żeby mieć cenę bazową z filmu)
            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == screeningId);

            if (screening == null)
                return false;

            var ticket = new Ticket
            {
                ScreeningId = screeningId,
                UserId = userId,
                Price = screening.Movie.BasePrice,
                SoldAt = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
