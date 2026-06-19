using ITFirma.Data;
using ITFirma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ITFirma.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger  = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.BrojKlijenata  = await _context.Klijenti.CountAsync();
            ViewBag.BrojProjekata  = await _context.Projekti.CountAsync();
            ViewBag.BrojZaposlenih = await _context.Zaposleni.CountAsync();
            ViewBag.BrojZadataka   = await _context.Zadaci.CountAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
