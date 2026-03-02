using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVCSQLite.Data;
using DemoMVCSQLite.Models;

namespace DemoMVCSQLite.Controllers
{
    public class ReservationTableController : Controller
    {
        private readonly AppDbContext _context;

        public ReservationTableController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            int pageSize = 10;
            var total = await _context.ReservationsTables.CountAsync();
            var reservations = await _context.ReservationsTables
                .Include(rt => rt.Client)
                .Include(rt => rt.TableVIP)
                    .ThenInclude(t => t.Venue)
                .Include(rt => rt.Event)
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
            var reservation = await _context.ReservationsTables
                .Include(rt => rt.Client)
                .Include(rt => rt.TableVIP)
                    .ThenInclude(t => t.Venue)
                .Include(rt => rt.Event)
                .FirstOrDefaultAsync(rt => rt.ReservationTableId == id);

            if (reservation == null) return NotFound();
            return View(reservation);
        }

        public IActionResult Create()
        {
            ViewBag.Clients = new SelectList(_context.Clients.Select(c => new {
                c.ClientId,
                NomComplet = c.Prenom + " " + c.Nom
            }), "ClientId", "NomComplet");

            ViewBag.Tables = new SelectList(_context.TablesVIP.Include(t => t.Venue).Select(t => new {
                t.TableVIPId,
                Label = t.Venue.Nom + " — Table " + t.Numero + " (" + t.NombrePlaces + " places)"
            }), "TableVIPId", "Label");

            ViewBag.Events = new SelectList(_context.Events, "EventId", "Titre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationTable reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.DateReservation = DateTime.Now;
                _context.ReservationsTables.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = new SelectList(_context.Clients.Select(c => new {
                c.ClientId,
                NomComplet = c.Prenom + " " + c.Nom
            }), "ClientId", "NomComplet");
            ViewBag.Tables = new SelectList(_context.TablesVIP.Include(t => t.Venue).Select(t => new {
                t.TableVIPId,
                Label = t.Venue.Nom + " — Table " + t.Numero + " (" + t.NombrePlaces + " places)"
            }), "TableVIPId", "Label");
            ViewBag.Events = new SelectList(_context.Events, "EventId", "Titre");
            return View(reservation);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var reservation = await _context.ReservationsTables.FindAsync(id);
            if (reservation == null) return NotFound();

            ViewBag.Clients = new SelectList(_context.Clients.Select(c => new {
                c.ClientId,
                NomComplet = c.Prenom + " " + c.Nom
            }), "ClientId", "NomComplet", reservation.ClientId);
            ViewBag.Tables = new SelectList(_context.TablesVIP.Include(t => t.Venue).Select(t => new {
                t.TableVIPId,
                Label = t.Venue.Nom + " — Table " + t.Numero + " (" + t.NombrePlaces + " places)"
            }), "TableVIPId", "Label", reservation.TableVIPId);
            ViewBag.Events = new SelectList(_context.Events, "EventId", "Titre", reservation.EventId);
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservationTable reservation)
        {
            if (id != reservation.ReservationTableId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.ReservationsTables.Update(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = new SelectList(_context.Clients.Select(c => new {
                c.ClientId,
                NomComplet = c.Prenom + " " + c.Nom
            }), "ClientId", "NomComplet", reservation.ClientId);
            ViewBag.Tables = new SelectList(_context.TablesVIP.Include(t => t.Venue).Select(t => new {
                t.TableVIPId,
                Label = t.Venue.Nom + " — Table " + t.Numero + " (" + t.NombrePlaces + " places)"
            }), "TableVIPId", "Label", reservation.TableVIPId);
            ViewBag.Events = new SelectList(_context.Events, "EventId", "Titre", reservation.EventId);
            return View(reservation);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.ReservationsTables
                .Include(rt => rt.Client)
                .Include(rt => rt.TableVIP)
                .Include(rt => rt.Event)
                .FirstOrDefaultAsync(rt => rt.ReservationTableId == id);

            if (reservation == null) return NotFound();
            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.ReservationsTables.FindAsync(id);
            if (reservation != null)
            {
                _context.ReservationsTables.Remove(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}