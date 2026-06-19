using ITFirma.Data;
using ITFirma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITFirma.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ZaposleniController : Controller
    {
        private readonly AppDbContext _context;

        public ZaposleniController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Zaposleni
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            var zaposleni = await _context.Zaposleni
                .OrderBy(z => z.Prezime)
                .ThenBy(z => z.Ime)
                .ToListAsync();
            return View(zaposleni);
        }

        // GET: Zaposleni/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var zaposleni = await _context.Zaposleni
                .Include(z => z.Angazovanja)
                    .ThenInclude(a => a.Projekat)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zaposleni == null) return NotFound();

            return View(zaposleni);
        }

        // GET: Zaposleni/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Zaposleni/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Ime,Prezime,Email,Pozicija")] Zaposleni zaposleni)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zaposleni);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Zaposleni \"{zaposleni.PunoIme}\" je uspešno dodan.";
                return RedirectToAction(nameof(Index));
            }
            return View(zaposleni);
        }

        // GET: Zaposleni/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var zaposleni = await _context.Zaposleni.FindAsync(id);
            if (zaposleni == null) return NotFound();

            return View(zaposleni);
        }

        // POST: Zaposleni/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Ime,Prezime,Email,Pozicija")] Zaposleni zaposleni)
        {
            if (id != zaposleni.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zaposleni);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Zaposleni \"{zaposleni.PunoIme}\" je uspešno ažuriran.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Zaposleni.Any(z => z.Id == zaposleni.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(zaposleni);
        }

        // GET: Zaposleni/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var zaposleni = await _context.Zaposleni
                .Include(z => z.Angazovanja)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zaposleni == null) return NotFound();

            return View(zaposleni);
        }

        // POST: Zaposleni/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zaposleni = await _context.Zaposleni.FindAsync(id);
            if (zaposleni != null)
            {
                _context.Zaposleni.Remove(zaposleni);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Zaposleni \"{zaposleni.PunoIme}\" je obrisan.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
