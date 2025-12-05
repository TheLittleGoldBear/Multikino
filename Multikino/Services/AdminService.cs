using Microsoft.EntityFrameworkCore;
using Multikino.Data;
using Multikino.Models;

namespace Multikino.Services
{
    public class AdminService : IAdminService
    {
        private readonly MultikinoDbContext _context;

        public AdminService(MultikinoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetMoviesAsync(string? search = null, string? sortOrder = null)
        {
            IQueryable<Movie> query = _context.Movies;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.Title.Contains(search));
            }

            query = sortOrder switch
            {
                "title_desc" => query.OrderByDescending(m => m.Title),
                "price" => query.OrderBy(m => m.BasePrice),
                "price_desc" => query.OrderByDescending(m => m.BasePrice),
                "release" => query.OrderBy(m => m.ReleaseDate),
                "release_desc" => query.OrderByDescending(m => m.ReleaseDate),
                _ => query.OrderBy(m => m.Title)
            };

            return await query.ToListAsync();
        }

        public async Task<Movie?> GetMovieAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<bool> UpdateMovieAsync(Movie movie)
        {
            var existing = await _context.Movies.FindAsync(movie.Id);
            if (existing == null) return false;

            existing.Title = movie.Title;
            existing.Description = movie.Description;
            existing.DurationMin = movie.DurationMin;
            existing.ReleaseDate = movie.ReleaseDate;
            existing.BasePrice = movie.BasePrice;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return false;

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Hall>> GetHallsAsync(string? search = null, string? sortOrder = null)
        {
            IQueryable<Hall> query = _context.Halls;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(h => h.Name.Contains(search));
            }

            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(h => h.Name),
                "capacity" => query.OrderBy(h => h.Capacity),
                "capacity_desc" => query.OrderByDescending(h => h.Capacity),
                _ => query.OrderBy(h => h.Name)
            };

            return await query.ToListAsync();
        }

        public async Task<Hall?> GetHallAsync(int id)
        {
            return await _context.Halls.FindAsync(id);
        }

        public async Task<Hall> CreateHallAsync(Hall hall)
        {
            _context.Halls.Add(hall);
            await _context.SaveChangesAsync();
            return hall;
        }

        public async Task<bool> UpdateHallAsync(Hall hall)
        {
            var existing = await _context.Halls.FindAsync(hall.Id);
            if (existing == null) return false;

            existing.Name = hall.Name;
            existing.Capacity = hall.Capacity;
            existing.Is3D = hall.Is3D;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHallAsync(int id)
        {
            var hall = await _context.Halls.FindAsync(id);
            if (hall == null) return false;

            _context.Halls.Remove(hall);
            await _context.SaveChangesAsync();
            return true;
        }

        // ================== SCREENINGS ==================

        public async Task<List<Screening>> GetScreeningsAsync(string? search = null, string? sortOrder = null)
        {
            IQueryable<Screening> query = _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Hall);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    (s.Movie != null && s.Movie.Title.Contains(search)) ||
                    (s.Hall != null && s.Hall.Name.Contains(search)) ||
                    s.Language.Contains(search));
            }

            query = sortOrder switch
            {
                "date_desc" => query.OrderByDescending(s => s.StartTime),
                "movie" => query.OrderBy(s => s.Movie!.Title),
                "movie_desc" => query.OrderByDescending(s => s.Movie!.Title),
                "hall" => query.OrderBy(s => s.Hall!.Name),
                "hall_desc" => query.OrderByDescending(s => s.Hall!.Name),
                _ => query.OrderBy(s => s.StartTime)
            };

            return await query.ToListAsync();
        }


        public async Task<Screening?> GetScreeningAsync(int id)
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Screening> CreateScreeningAsync(Screening screening)
        {
            _context.Screenings.Add(screening);
            await _context.SaveChangesAsync();
            return screening;
        }

        public async Task<bool> UpdateScreeningAsync(Screening screening)
        {
            var existing = await _context.Screenings.FindAsync(screening.Id);
            if (existing == null) return false;

            existing.MovieId = screening.MovieId;
            existing.HallId = screening.HallId;
            existing.StartTime = screening.StartTime;
            existing.Language = screening.Language;
            existing.Is3D = screening.Is3D;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteScreeningAsync(int id)
        {
            var screening = await _context.Screenings.FindAsync(id);
            if (screening == null) return false;

            _context.Screenings.Remove(screening);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
