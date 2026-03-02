using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVCSQLite.Data;
using DemoMVCSQLite.Models;

namespace DemoMVCSQLite.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;

        public ReservationController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            int pageSize = 10;
            var total = await _context.Reservations.CountAsync();
            var reservations = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.Event)
                .Skip((pg - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = pg;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Total = total;

            return View(reservations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.Event)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null) return NotFound();
            return View(reservation);
        }

        public IActionResult Create()
        {
            ViewBag.Clients = new SelectList(_context.Clients.Select(c => new {
                c.ClientId,
                NomComplet = c.Prenom + " " + c.Nom
            }), "ClientId", "NomComplet");
            ViewBag.Tickets = new SelectList(_context.Tickets.Include(t => t.Event).Select(t => new {
                t.TicketId,
                Label = t.Event.Titre + " — " + t.Type + " (" + t.Prix + "€)"
            }), "TicketId", "Label");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var ticket = await _context.Tickets.FindAsync(reservation.TicketId);
                if (ticket != null)
                {
                    reservation.DateReservation = DateTime.Now;
                    reservation.MontantTotal = ticket.Prix * reservation.QteTickets;
                    _context.Reservations.Add(reservation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Clients = new SelectList(_context.Clients.Select(c => new {
                c.ClientId,
                NomComplet = c.Prenom + " " + c.Nom
            }), "ClientId", "NomComplet");
            ViewBag.Tickets = new SelectList(_context.Tickets.Include(t => t.Event).Select(t => new {
                t.TicketId,
                Label = t.Event.Titre + " — " + t.Type + " (" + t.Prix + "€)"
            }), "TicketId", "Label");
            return View(reservation);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            ViewBag.Clients = new SelectList(_context.Clients.Select(c => new {
                c.ClientId,
                NomComplet = c.Prenom + " " + c.Nom
            }), "ClientId", "NomComplet", reservation.ClientId);
            ViewBag.Tickets = new SelectList(_context.Tickets.Include(t => t.Event).Select(t => new {
                t.TicketId,
                Label = t.Event.Titre + " — " + t.Type + " (" + t.Prix + "€)"
            }), "TicketId", "Label", reservation.TicketId);
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation reservation)
        {
            if (id != reservation.ReservationId) return NotFound();

            if (ModelState.IsValid)
            {
                var ticket = await _context.Tickets.FindAsync(reservation.TicketId);
                if (ticket != null)
                {
                    reservation.MontantTotal = ticket.Prix * reservation.QteTickets;
                    _context.Reservations.Update(reservation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Clients = new SelectList(_context.Clients.Select(c => new {
                c.ClientId,
                NomComplet = c.Prenom + " " + c.Nom
            }), "ClientId", "NomComplet", reservation.ClientId);
            ViewBag.Tickets = new SelectList(_context.Tickets.Include(t => t.Event).Select(t => new {
                t.TicketId,
                Label = t.Event.Titre + " — " + t.Type + " (" + t.Prix + "€)"
            }), "TicketId", "Label", reservation.TicketId);
            return View(reservation);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Ticket)
                    .ThenInclude(t => t.Event)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null) return NotFound();
            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}