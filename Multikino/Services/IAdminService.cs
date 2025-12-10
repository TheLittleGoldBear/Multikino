using Multikino.Models;
using Multikino.Models.View;

namespace Multikino.Services
{
    public interface IAdminService
    {
        Task<List<Movie>> GetMoviesAsync(string? search = null, string? sortOrder = null);
        Task<Movie?> GetMovieAsync(int id);
        Task<Movie> CreateMovieAsync(Movie movie);
        Task<bool> UpdateMovieAsync(Movie movie);
        Task<bool> DeleteMovieAsync(int id);

        Task<List<Hall>> GetHallsAsync(string? search = null, string? sortOrder = null);
        Task<Hall?> GetHallAsync(int id);
        Task<Hall> CreateHallAsync(Hall hall);
        Task<bool> UpdateHallAsync(Hall hall);
        Task<bool> DeleteHallAsync(int id);

        Task<List<Screening>> GetScreeningsAsync(string? search = null, string? sortOrder = null);
        Task<Screening?> GetScreeningAsync(int id);
        Task<Screening> CreateScreeningAsync(Screening screening);
        Task<bool> UpdateScreeningAsync(Screening screening);
        Task<bool> DeleteScreeningAsync(int id);
        Task<IEnumerable<TicketSalesReportItem>> GetTicketSalesReportAsync(DateTime from, DateTime to);
        Task<IEnumerable<RevenueByMovieItem>> GetRevenueByMovieAsync(DateTime from, DateTime to);

    }
}
