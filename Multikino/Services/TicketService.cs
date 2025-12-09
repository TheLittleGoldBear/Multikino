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
            var q = _db.Screenings
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Where(s => s.StartTime >= DateTime.UtcNow)
            .AsQueryable();


            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                q = q.Where(s => s.Movie.Title.ToLower().Contains(search) || s.Hall.Name.ToLower().Contains(search));
            }


            q = sortOrder switch
            {
                "date_desc" => q.OrderByDescending(s => s.StartTime),
                "movie" => q.OrderBy(s => s.Movie.Title),
                "movie_desc" => q.OrderByDescending(s => s.Movie.Title),
                _ => q.OrderBy(s => s.StartTime)
            };


            return await q.ToListAsync();
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