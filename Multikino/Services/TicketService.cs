using Microsoft.EntityFrameworkCore;
using Multikino.Data;
using Multikino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multikino.Services
{
    public class TicketService : ITicketService
    {
        private readonly MultikinoDbContext _db;


        public TicketService(MultikinoDbContext db)
        {
            _db = db;
        }


        public async Task<Screening?> GetScreeningAsync(int id)
        {
            return await _db.Screenings
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Include(s => s.Tickets)
            .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Screening>> GetUpcomingScreeningsAsync(string? search = null, string? sortOrder = null)
        {
            // Bazowe zapytanie z potrzebnymi Include (jeśli chcesz mieć związane encje od razu w wynikach)
            var baseQuery = _db.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                // nie musimy Include(s => s.Tickets) bo liczymy Tickets przez oddzielne zapytanie do tabeli Tickets
                .Where(s => s.StartTime >= DateTime.UtcNow)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.Trim().ToLower();
                baseQuery = baseQuery.Where(s =>
                    s.Movie.Title.ToLower().Contains(lowered) ||
                    s.Hall.Name.ToLower().Contains(lowered));
            }

            // Projekcja pomocnicza — dodajemy pole Free, które EF potrafi przetłumaczyć: subquery Count(...) na Tickets
            var projected = baseQuery.Select(s => new
            {
                Screening = s,
                Free = s.Hall.Capacity - _db.Tickets.Count(t => t.ScreeningId == s.Id) // translatowalne do SQL
            });

            // Sortowanie na podstawie sortOrder
            projected = sortOrder switch
            {
                "date_desc" => projected.OrderByDescending(p => p.Screening.StartTime),
                "movie" => projected.OrderBy(p => p.Screening.Movie.Title),
                "movie_desc" => projected.OrderByDescending(p => p.Screening.Movie.Title),
                "hall" => projected.OrderBy(p => p.Screening.Hall.Name),
                "hall_desc" => projected.OrderByDescending(p => p.Screening.Hall.Name),

                // language
                "language" => projected.OrderBy(p => p.Screening.Language),
                "language_desc" => projected.OrderByDescending(p => p.Screening.Language),

                // is3d (proste bool ordering — EF przetłumaczy)
                "is3d" => projected.OrderBy(p => p.Screening.Is3D),
                "is3d_desc" => projected.OrderByDescending(p => p.Screening.Is3D),

                // wolne miejsca
                "free" => projected.OrderBy(p => p.Free),
                "free_desc" => projected.OrderByDescending(p => p.Free),

                // domyślnie data rosnąco
                _ => projected.OrderBy(p => p.Screening.StartTime)
            };

            // Pobieramy listę Screening'ów (wyciągamy z projekcji)
            var list = await projected.Select(p => p.Screening).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsForUserAsync(int userId)
        {
            return await _db.Tickets
            .Include(t => t.Screening)
            .ThenInclude(s => s.Movie)
            .Include(t => t.Screening)
            .ThenInclude(s => s.Hall)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.SoldAt)
            .ToListAsync();
        }


        public async Task<(bool Success, string? ErrorMessage)> PurchaseTicketAsync(int screeningId, int? userId)
        {
            var screening = await _db.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.Id == screeningId);

            if (screening == null)
                return (false, "Seans nie istnieje.");

            if (screening.StartTime <= DateTime.UtcNow)
                return (false, "Seans już się rozpoczął lub minął.");

            var soldCount = screening.Tickets.Count;
            if (soldCount >= screening.Hall.Capacity)
                return (false, "Brak wolnych miejsc w tej sali.");

            decimal price = screening.Movie.BasePrice;
            if (screening.Is3D || screening.Hall.Is3D)
                price += 5m;

            var ticket = new Ticket
            {
                ScreeningId = screening.Id,
                UserId = userId,
                Price = price,
                SoldAt = DateTime.UtcNow,
            };

            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            return (true, null);
        }

    }
}