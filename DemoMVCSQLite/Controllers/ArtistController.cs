using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVCSQLite.Data;
using DemoMVCSQLite.Models;

namespace DemoMVCSQLite.Controllers
{
    public class ArtistController : Controller
    {
        private readonly AppDbContext _context;

        public ArtistController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var artists = await _context.Artists.ToListAsync();
            return View(artists);
        }

        public async Task<IActionResult> Details(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.EventArtists)
                    .ThenInclude(ea => ea.Event)
                .FirstOrDefaultAsync(a => a.ArtistId == id);

            if (artist == null) return NotFound();
            return View(artist);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Artist artist)
        {
            if (ModelState.IsValid)
            {
                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Artist artist)
        {
            if (id != artist.ArtistId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Artists.Update(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}