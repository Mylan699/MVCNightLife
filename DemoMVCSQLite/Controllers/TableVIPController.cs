using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVCSQLite.Data;
using DemoMVCSQLite.Models;

namespace DemoMVCSQLite.Controllers
{
    public class TableVIPController : Controller
    {
        private readonly AppDbContext _context;

        public TableVIPController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            int pageSize = 10;
            var total = await _context.TablesVIP.CountAsync();
            var tables = await _context.TablesVIP
                .Include(t => t.Venue)
                .Skip((pg - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = pg;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Total = total;

            return View(tables);
        }

        public async Task<IActionResult> Details(int id)
        {
            var table = await _context.TablesVIP
                .Include(t => t.Venue)
                .Include(t => t.ReservationsTables)
                    .ThenInclude(rt => rt.Client)
                .Include(t => t.ReservationsTables)
                    .ThenInclude(rt => rt.Event)
                .FirstOrDefaultAsync(t => t.TableVIPId == id);

            if (table == null) return NotFound();
            return View(table);
        }

        public IActionResult Create()
        {
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "Nom");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TableVIP table)
        {
            if (ModelState.IsValid)
            {
                _context.TablesVIP.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "Nom");
            return View(table);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var table = await _context.TablesVIP.FindAsync(id);
            if (table == null) return NotFound();
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "Nom", table.VenueId);
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TableVIP table)
        {
            if (id != table.TableVIPId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.TablesVIP.Update(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Venues = new SelectList(_context.Venues, "VenueId", "Nom", table.VenueId);
            return View(table);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var table = await _context.TablesVIP
                .Include(t => t.Venue)
                .FirstOrDefaultAsync(t => t.TableVIPId == id);
            if (table == null) return NotFound();
            return View(table);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _context.TablesVIP.FindAsync(id);
            if (table != null)
            {
                _context.TablesVIP.Remove(table);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}