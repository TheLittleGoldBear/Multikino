using Multikino.Models;

namespace Multikino.Services
{
    public interface ICinemaService
    {
        Task<List<Movie>> GetMoviesAsync(string? search, string? sortOrder);
        Task<Movie?> GetMovieWithScreeningsAsync(int movieId);

        Task<List<Screening>> GetScreeningsAsync(int movieId, DateTime? from, DateTime? to);
        Task<bool> ReserveTicketAsync(int screeningId, int? userId = null);
    }
}
