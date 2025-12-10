using Multikino.Models;

namespace Multikino.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<Screening>> GetUpcomingScreeningsAsync(string? search = null, string? sortOrder = null);
        Task<Screening?> GetScreeningAsync(int id);
        Task<IEnumerable<Ticket>> GetTicketsForUserAsync(int userId, string? search = null, string? sortOrder = null);
        Task<(bool Success, string? ErrorMessage)> PurchaseTicketAsync(int screeningId, int? userId);
    }
}