using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVCSQLite.Data;
using DemoMVCSQLite.Models;

namespace DemoMVCSQLite.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            int pageSize = 10;
            var total = await _context.Events.CountAsync();
            var events = await _context.Events
                .Include(e => e.Venue)
                .Skip((pg - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = pg;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Total = total;

            return View(events);
        }

        public async Task<IActionResult> Details(int id, int pg = 1)
        {
            int pageSize = 10;

            var ev = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventArtists)
                    .ThenInclude(ea => ea.Artist)
                .Include(e => e.Tickets)
                    .ThenInclude(t => t.Reservations)
                        .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound();

            var reservations = ev.Tickets.SelectMany(t => t.Reservations).ToList();
            int totalClients = reservations.Count;
            int totalPages = (int)Math.Ceiling(totalClients / (double)pageSize);

            var reservationsPaged = reservations
                .Skip((pg - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = pg;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalClients = totalClients;
            ViewBag.Total = totalClients;
            ViewBag.ReservationsPaged = reservationsPaged;

            return View(ev);
        }

        public IActionResult Create()
        {
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "Nom");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event ev)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "Nom");
            return View(ev);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "Nom", ev.VenueId);
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event ev)
        {
            if (id != ev.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Events.Update(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "Nom", ev.VenueId);
            return View(ev);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ImportClients(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportClients(int id, string jsonData)
        {
            var ev = await _context.Events
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound();

            try
            {
                var clients = System.Text.Json.JsonSerializer.Deserialize<List<Client>>(jsonData);

                if (clients == null || !clients.Any())
                {
                    TempData["Error"] = "Aucun client trouvé dans le JSON.";
                    return RedirectToAction(nameof(ImportClients), new { id });
                }

                var ticket = ev.Tickets.FirstOrDefault();
                int nouveaux = 0;
                int dejaExistants = 0;
                int dejaInscrits = 0;

                foreach (var client in clients)
                {
                    var existingClient = await _context.Clients
                        .FirstOrDefaultAsync(c => c.Email == client.Email);

                    if (existingClient == null)
                    {
                        _context.Clients.Add(client);
                        await _context.SaveChangesAsync();
                        existingClient = client;
                        nouveaux++;
                    }
                    else
                    {
                        dejaExistants++;
                    }

                    if (ticket != null)
                    {
                        var dejaReserve = await _context.Reservations
                            .AnyAsync(r => r.ClientId == existingClient.ClientId && r.TicketId == ticket.TicketId);

                        if (!dejaReserve)
                        {
                            var reservation = new Reservation
                            {
                                ClientId = existingClient.ClientId,
                                TicketId = ticket.TicketId,
                                DateReservation = DateTime.Now,
                                Statut = StatutReservation.Confirmee,
                                QteTickets = 1,
                                MontantTotal = ticket.Prix
                            };
                            _context.Reservations.Add(reservation);
                        }
                        else
                        {
                            dejaInscrits++;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"{nouveaux} nouveau(x) client(s) créé(s), " +
                                      $"{dejaExistants} déjà existant(s), " +
                                      $"{dejaInscrits} déjà inscrit(s) à cette soirée.";
            }
            catch
            {
                TempData["Error"] = "Format JSON invalide.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}