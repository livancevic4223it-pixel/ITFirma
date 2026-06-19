using ITFirma.Data;
using ITFirma.Models;
using ITFirma.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITFirma.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProjekatController : Controller
    {
        private readonly AppDbContext _context;

        public ProjekatController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Projekat
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index(int page = 1, string? sort = null, string? filter = null)
        {
            const int pageSize = 5;

            var query = _context.Projekti
                .Include(p => p.Klijent)
                .AsQueryable();

            // ── Filter po statusu ───────────────────────────────────────
            if (!string.IsNullOrEmpty(filter) &&
                Enum.TryParse<StatusProjekta>(filter, out var statusFilter))
            {
                query = query.Where(p => p.Status == statusFilter);
            }

            // ── Sortiranje ──────────────────────────────────────────────
            query = sort switch
            {
                "naziv"        => query.OrderBy(p => p.Naziv),
                "naziv_desc"   => query.OrderByDescending(p => p.Naziv),
                "klijent"      => query.OrderBy(p => p.Klijent!.Naziv),
                "klijent_desc" => query.OrderByDescending(p => p.Klijent!.Naziv),
                "datum_desc"   => query.OrderByDescending(p => p.DatumPocetka),
                "status"       => query.OrderBy(p => p.Status),
                "status_desc"  => query.OrderByDescending(p => p.Status),
                _              => query.OrderBy(p => p.DatumPocetka), // default: datum ASC
            };

            // ── Paginacija ──────────────────────────────────────────────
            var total = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            page = Math.Max(1, Math.Min(page, totalPages));

            var projekti = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new ProjekatIndexVM
            {
                Projekti    = projekti,
                CurrentPage = page,
                TotalPages  = totalPages,
                PageSize    = pageSize,
                Sort        = sort,
                Filter      = filter,
                TotalCount  = total
            };

            return View(vm);
        }

        // GET: Projekat/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var projekat = await _context.Projekti
                .Include(p => p.Klijent)
                .Include(p => p.Angazovanja)
                    .ThenInclude(a => a.Zaposleni)
                .Include(p => p.Zadaci)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projekat == null) return NotFound();

            return View(projekat);
        }

        // GET: Projekat/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Projekat/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Naziv,Opis,DatumPocetka,DatumZavrsetka,Status,KlijentId")] Projekat projekat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projekat);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Projekat \"{projekat.Naziv}\" je uspešno kreiran.";
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(projekat.KlijentId, projekat.Status);
            return View(projekat);
        }

        // GET: Projekat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var projekat = await _context.Projekti.FindAsync(id);
            if (projekat == null) return NotFound();

            PopulateDropdowns(projekat.KlijentId, projekat.Status);
            return View(projekat);
        }

        // POST: Projekat/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Naziv,Opis,DatumPocetka,DatumZavrsetka,Status,KlijentId")] Projekat projekat)
        {
            if (id != projekat.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projekat);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Projekat \"{projekat.Naziv}\" je uspešno ažuriran.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Projekti.Any(p => p.Id == projekat.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(projekat.KlijentId, projekat.Status);
            return View(projekat);
        }

        // GET: Projekat/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var projekat = await _context.Projekti
                .Include(p => p.Klijent)
                .Include(p => p.Zadaci)
                .Include(p => p.Angazovanja)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projekat == null) return NotFound();

            return View(projekat);
        }

        // POST: Projekat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projekat = await _context.Projekti.FindAsync(id);
            if (projekat != null)
            {
                _context.Projekti.Remove(projekat);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Projekat \"{projekat.Naziv}\" je obrisan.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ───────────────────────────────────────────────────
        private void PopulateDropdowns(int? selectedKlijentId = null,
                                       StatusProjekta selectedStatus = StatusProjekta.Planiran)
        {
            ViewBag.KlijentId = new SelectList(
                _context.Klijenti.OrderBy(k => k.Naziv),
                "Id", "Naziv",
                selectedKlijentId);

            ViewBag.StatusList = new SelectList(
                Enum.GetValues<StatusProjekta>()
                    .Select(s => new { Value = (int)s, Text = s.ToString() }),
                "Value", "Text",
                (int)selectedStatus);
        }
    }
}
