using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Multikino.Models.View;
using Multikino.Services;
using System.Security.Claims;


namespace Multikino.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IActionResult> MyTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Challenge();

            var tickets = await _ticketService.GetTicketsForUserAsync(userId);
            var vm = new UserTicketsViewModel { Tickets = tickets };
            return View(vm);
        }
    }
}