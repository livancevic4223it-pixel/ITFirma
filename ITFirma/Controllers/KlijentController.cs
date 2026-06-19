using ITFirma.Data;
using ITFirma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITFirma.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KlijentController : Controller
    {
        private readonly AppDbContext _context;

        public KlijentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Klijent
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var klijenti = await _context.Klijenti
                .OrderBy(k => k.Naziv)
                .ToListAsync();
            return View(klijenti);
        }

        // GET: Klijent/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var klijent = await _context.Klijenti
                .Include(k => k.Projekti)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (klijent == null) return NotFound();

            return View(klijent);
        }

        // GET: Klijent/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Klijent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Naziv,Email,Telefon,Adresa")] Klijent klijent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(klijent);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Klijent \"{klijent.Naziv}\" je uspešno kreiran.";
                return RedirectToAction(nameof(Index));
            }
            return View(klijent);
        }

        // GET: Klijent/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var klijent = await _context.Klijenti.FindAsync(id);
            if (klijent == null) return NotFound();

            return View(klijent);
        }

        // POST: Klijent/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Naziv,Email,Telefon,Adresa")] Klijent klijent)
        {
            if (id != klijent.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(klijent);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Klijent \"{klijent.Naziv}\" je uspešno ažuriran.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Klijenti.Any(k => k.Id == klijent.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(klijent);
        }

        // GET: Klijent/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var klijent = await _context.Klijenti
                .Include(k => k.Projekti)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (klijent == null) return NotFound();

            return View(klijent);
        }

        // POST: Klijent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var klijent = await _context.Klijenti.FindAsync(id);
            if (klijent != null)
            {
                _context.Klijenti.Remove(klijent);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Klijent \"{klijent.Naziv}\" je obrisan.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
