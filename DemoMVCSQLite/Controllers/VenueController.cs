using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVCSQLite.Data;
using DemoMVCSQLite.Models;

namespace DemoMVCSQLite.Controllers
{
    public class VenueController : Controller
    {
        private readonly AppDbContext _context;

        public VenueController(AppDbContext context)
        {
            _context = context;
        }

        // Liste de tous les établissements
        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venues.ToListAsync();
            return View(venues);
        }

        // Détail d'un établissement
        public async Task<IActionResult> Details(int id)
        {
            var venue = await _context.Venues
                .Include(v => v.Events)
                .Include(v => v.TablesVIP)
                .FirstOrDefaultAsync(v => v.VenueId == id);

            if (venue == null) return NotFound();
            return View(venue);
        }

        // Formulaire création
        public IActionResult Create()
        {
            return View();
        }

        // Sauvegarder création
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Venues.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // Formulaire modification
        public async Task<IActionResult> Edit(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // Sauvegarder modification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Venues.Update(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // Confirmation suppression
        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // Supprimer
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}