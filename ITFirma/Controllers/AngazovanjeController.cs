using ITFirma.Data;
using ITFirma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITFirma.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AngazovanjeController : Controller
    {
        private readonly AppDbContext _context;

        private static readonly List<string> Uloge = new() { "Dev", "Lead", "QA" };

        public AngazovanjeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Angazovanje/Create?projekatId=5
        public async Task<IActionResult> Create(int projekatId)
        {
            var projekat = await _context.Projekti
                .Include(p => p.Angazovanja)
                .FirstOrDefaultAsync(p => p.Id == projekatId);

            if (projekat == null) return NotFound();

            // Isključi zaposlene koji su već angažovani na ovom projektu
            var vecAngazovaniIds = projekat.Angazovanja.Select(a => a.ZaposleniId).ToHashSet();
            var slobodni = await _context.Zaposleni
                .Where(z => !vecAngazovaniIds.Contains(z.Id))
                .OrderBy(z => z.Prezime)
                .ToListAsync();

            if (!slobodni.Any())
            {
                TempData["Warning"] = "Svi zaposleni su već angažovani na ovom projektu.";
                return RedirectToAction("Details", "Projekat", new { id = projekatId });
            }

            ViewBag.ProjekatId = projekatId;
            ViewBag.ProjekatNaziv = projekat.Naziv;
            ViewBag.ZaposleniId = new SelectList(
                slobodni.Select(z => new { z.Id, PunoIme = $"{z.Prezime} {z.Ime} — {z.Pozicija}" }),
                "Id", "PunoIme");
            ViewBag.UlogaList = new SelectList(Uloge);

            return View(new Angazovanje { ProjekatId = projekatId, DatumOd = DateTime.Today });
        }

        // POST: Angazovanje/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProjekatId,ZaposleniId,Uloga,Satnica,DatumOd")] Angazovanje angazovanje)
        {
            // Provjeri duplikat (composite PK)
            var postoji = await _context.Angazovanja
                .AnyAsync(a => a.ProjekatId == angazovanje.ProjekatId
                            && a.ZaposleniId == angazovanje.ZaposleniId);
            if (postoji)
            {
                ModelState.AddModelError("ZaposleniId",
                    "Odabrani zaposleni je već angažovan na ovom projektu.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(angazovanje);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Angažovanje je uspešno dodano.";
                return RedirectToAction("Details", "Projekat", new { id = angazovanje.ProjekatId });
            }

            // Ponovo napuni dropdowne
            var projekat = await _context.Projekti
                .Include(p => p.Angazovanja)
                .FirstOrDefaultAsync(p => p.Id == angazovanje.ProjekatId);

            var vecAngazovaniIds = projekat!.Angazovanja
                .Where(a => a.ZaposleniId != angazovanje.ZaposleniId)
                .Select(a => a.ZaposleniId).ToHashSet();

            var slobodni = await _context.Zaposleni
                .Where(z => !vecAngazovaniIds.Contains(z.Id))
                .OrderBy(z => z.Prezime)
                .ToListAsync();

            ViewBag.ProjekatId = angazovanje.ProjekatId;
            ViewBag.ProjekatNaziv = projekat.Naziv;
            ViewBag.ZaposleniId = new SelectList(
                slobodni.Select(z => new { z.Id, PunoIme = $"{z.Prezime} {z.Ime} — {z.Pozicija}" }),
                "Id", "PunoIme",
                angazovanje.ZaposleniId);
            ViewBag.UlogaList = new SelectList(Uloge, angazovanje.Uloga);

            return View(angazovanje);
        }

        // GET: Angazovanje/Edit?projekatId=5&zaposleniId=2
        public async Task<IActionResult> Edit(int projekatId, int zaposleniId)
        {
            var angazovanje = await _context.Angazovanja
                .Include(a => a.Projekat)
                .Include(a => a.Zaposleni)
                .FirstOrDefaultAsync(a => a.ProjekatId == projekatId
                                       && a.ZaposleniId == zaposleniId);

            if (angazovanje == null) return NotFound();

            ViewBag.UlogaList = new SelectList(Uloge, angazovanje.Uloga);
            return View(angazovanje);
        }

        // POST: Angazovanje/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("ProjekatId,ZaposleniId,Uloga,Satnica,DatumOd")] Angazovanje angazovanje)
        {
            var existing = await _context.Angazovanja
                .FirstOrDefaultAsync(a => a.ProjekatId == angazovanje.ProjekatId
                                       && a.ZaposleniId == angazovanje.ZaposleniId);

            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                existing.Uloga   = angazovanje.Uloga;
                existing.Satnica = angazovanje.Satnica;
                existing.DatumOd = angazovanje.DatumOd;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Angažovanje je uspješno ažurirano.";
                return RedirectToAction("Details", "Projekat", new { id = angazovanje.ProjekatId });
            }

            // Ponovo napuni navigacijska svojstva za view
            angazovanje.Projekat  = await _context.Projekti.FindAsync(angazovanje.ProjekatId);
            angazovanje.Zaposleni = await _context.Zaposleni.FindAsync(angazovanje.ZaposleniId);
            ViewBag.UlogaList = new SelectList(Uloge, angazovanje.Uloga);

            return View(angazovanje);
        }

        // GET: Angazovanje/Delete?projekatId=5&zaposleniId=2
        public async Task<IActionResult> Delete(int projekatId, int zaposleniId)
        {
            var angazovanje = await _context.Angazovanja
                .Include(a => a.Projekat)
                .Include(a => a.Zaposleni)
                .FirstOrDefaultAsync(a => a.ProjekatId == projekatId
                                       && a.ZaposleniId == zaposleniId);

            if (angazovanje == null) return NotFound();

            return View(angazovanje);
        }

        // POST: Angazovanje/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int projekatId, int zaposleniId)
        {
            var angazovanje = await _context.Angazovanja
                .FirstOrDefaultAsync(a => a.ProjekatId == projekatId
                                       && a.ZaposleniId == zaposleniId);

            if (angazovanje != null)
            {
                _context.Angazovanja.Remove(angazovanje);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Angažovanje je uklonjeno.";
            }

            return RedirectToAction("Details", "Projekat", new { id = projekatId });
        }
    }
}
