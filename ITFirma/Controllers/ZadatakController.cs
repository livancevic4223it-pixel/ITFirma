using ITFirma.Data;
using ITFirma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITFirma.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ZadatakController : Controller
    {
        private readonly AppDbContext _context;

        public ZadatakController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Zadatak
        public async Task<IActionResult> Index()
        {
            var zadaci = await _context.Zadaci
                .Include(z => z.Projekat)
                .OrderBy(z => z.Rok)
                .ToListAsync();
            return View(zadaci);
        }

        // GET: Zadatak/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var zadatak = await _context.Zadaci
                .Include(z => z.Projekat)
                    .ThenInclude(p => p!.Klijent)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zadatak == null) return NotFound();

            return View(zadatak);
        }

        // GET: Zadatak/Create
        public IActionResult Create(int? projekatId)
        {
            PopulateDropdowns(projekatId);
            return View();
        }

        // POST: Zadatak/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Naziv,Opis,Rok,Status,ProjekatId")] Zadatak zadatak)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zadatak);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Zadatak \"{zadatak.Naziv}\" je uspešno kreiran.";
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(zadatak.ProjekatId);
            return View(zadatak);
        }

        // GET: Zadatak/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var zadatak = await _context.Zadaci.FindAsync(id);
            if (zadatak == null) return NotFound();

            PopulateDropdowns(zadatak.ProjekatId, zadatak.Status);
            return View(zadatak);
        }

        // POST: Zadatak/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Naziv,Opis,Rok,Status,ProjekatId")] Zadatak zadatak)
        {
            if (id != zadatak.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zadatak);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Zadatak \"{zadatak.Naziv}\" je uspešno ažuriran.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Zadaci.Any(z => z.Id == zadatak.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(zadatak.ProjekatId, zadatak.Status);
            return View(zadatak);
        }

        // GET: Zadatak/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var zadatak = await _context.Zadaci
                .Include(z => z.Projekat)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zadatak == null) return NotFound();

            return View(zadatak);
        }

        // POST: Zadatak/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zadatak = await _context.Zadaci.FindAsync(id);
            if (zadatak != null)
            {
                _context.Zadaci.Remove(zadatak);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Zadatak \"{zadatak.Naziv}\" je obrisan.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ───────────────────────────────────────────────────
        private void PopulateDropdowns(int? selectedProjekatId = null,
                                       StatusZadatka selectedStatus = StatusZadatka.Otvoren)
        {
            ViewBag.ProjekatId = new SelectList(
                _context.Projekti.OrderBy(p => p.Naziv),
                "Id", "Naziv",
                selectedProjekatId);

            ViewBag.StatusList = new SelectList(
                Enum.GetValues<StatusZadatka>()
                    .Select(s => new { Value = (int)s, Text = s.ToString() }),
                "Value", "Text",
                (int)selectedStatus);
        }
    }
}
