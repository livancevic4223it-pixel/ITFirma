using ITFirma.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITFirma.Data
{
    public static class SeedData
    {
        // ── Uloge ─────────────────────────────────────────────────────
        public const string RoleAdmin = "Admin";
        public const string RoleUser  = "User";
        public const string RoleGuest = "Guest";

        public static async Task SeedIdentityAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seeduj uloge
            foreach (var role in new[] { RoleGuest, RoleUser, RoleAdmin })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seeduj admin usera
            const string adminEmail = "admin@itfirma.com";
            const string adminPass  = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email    = adminEmail,
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(admin, adminPass);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, RoleAdmin);
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

            // Idempotentno — ako već ima podataka, preskočiti
            if (context.Klijenti.Any())
                return;

            // ── Klijenti ──────────────────────────────────────────────
            var klijenti = new List<Klijent>
            {
                new() { Naziv = "Nexo Solutions d.o.o.", Email = "info@nexo.ba",    Telefon = "+387 33 100 200", Adresa = "Zmaja od Bosne 7, Sarajevo" },
                new() { Naziv = "DataBridge GmbH",       Email = "contact@dbg.de",  Telefon = "+49 89 555 0101", Adresa = "Maximilianstraße 12, München" },
                new() { Naziv = "Adriatic Tech j.d.o.o.",Email = "hello@adritech.hr",Telefon = "+385 1 234 5678", Adresa = "Ilica 80, Zagreb" },
            };
            context.Klijenti.AddRange(klijenti);
            context.SaveChanges();

            // ── Zaposleni ─────────────────────────────────────────────
            var zaposleni = new List<Zaposleni>
            {
                new() { Ime = "Marko",  Prezime = "Petrović", Email = "marko.petrovic@itfirma.ba",  Pozicija = "Senior Developer" },
                new() { Ime = "Ana",    Prezime = "Kovač",    Email = "ana.kovac@itfirma.ba",        Pozicija = "QA Engineer" },
                new() { Ime = "Stefan", Prezime = "Đorđić",   Email = "stefan.djordic@itfirma.ba",  Pozicija = "Tech Lead" },
            };
            context.Zaposleni.AddRange(zaposleni);
            context.SaveChanges();

            // ── Projekti ──────────────────────────────────────────────
            var projekti = new List<Projekat>
            {
                new()
                {
                    Naziv          = "ERP Integracija",
                    Opis           = "Integracija ERP sistema sa legacy bazom podataka klijenta.",
                    DatumPocetka   = new DateTime(2024, 1, 15),
                    DatumZavrsetka = new DateTime(2024, 9, 30),
                    Status         = StatusProjekta.Zavrsen,
                    KlijentId      = klijenti[0].Id,
                },
                new()
                {
                    Naziv          = "Mobile Banking App",
                    Opis           = "Razvoj mobilne aplikacije za iOS i Android platformu.",
                    DatumPocetka   = new DateTime(2024, 6, 1),
                    DatumZavrsetka = new DateTime(2025, 3, 31),
                    Status         = StatusProjekta.UToku,
                    KlijentId      = klijenti[1].Id,
                },
                new()
                {
                    Naziv        = "E-Commerce Portal",
                    Opis         = "B2C web shop s integracijom plaćanja i logistike.",
                    DatumPocetka = new DateTime(2024, 10, 1),
                    Status       = StatusProjekta.Planiran,
                    KlijentId    = klijenti[2].Id,
                },
            };
            context.Projekti.AddRange(projekti);
            context.SaveChanges();

            // ── Zadaci ────────────────────────────────────────────────
            var zadaci = new List<Zadatak>
            {
                // Projekat 0 — ERP Integracija (završen)
                new() { Naziv = "Analiza postojeće baze",     Opis = "Dokumentovati shemu legacy baze.",          Rok = new DateTime(2024, 2, 15), Status = StatusZadatka.Zavrsen,  ProjekatId = projekti[0].Id },
                new() { Naziv = "Mapiranje entiteta",          Opis = "Kreirati mapping tablice.",                  Rok = new DateTime(2024, 3, 10), Status = StatusZadatka.Zavrsen,  ProjekatId = projekti[0].Id },
                new() { Naziv = "ETL pipeline",                Opis = "Implementirati ETL skripte.",               Rok = new DateTime(2024, 5, 31), Status = StatusZadatka.Zavrsen,  ProjekatId = projekti[0].Id },
                new() { Naziv = "Testiranje migracije",        Opis = "Regresijsko testiranje nakon migracije.",   Rok = new DateTime(2024, 8, 30), Status = StatusZadatka.Zavrsen,  ProjekatId = projekti[0].Id },

                // Projekat 1 — Mobile Banking App (u toku)
                new() { Naziv = "Dizajn UI/UX wireframes",    Opis = "Figma prototip svih ekrana.",               Rok = new DateTime(2024, 7, 15), Status = StatusZadatka.Zavrsen,  ProjekatId = projekti[1].Id },
                new() { Naziv = "Autentifikacija (JWT)",       Opis = "Login, refresh token, biometrika.",         Rok = new DateTime(2024, 9, 1),  Status = StatusZadatka.Zavrsen,  ProjekatId = projekti[1].Id },
                new() { Naziv = "Integracija payment gateway", Opis = "Stripe + lokalni bankarski API.",           Rok = new DateTime(2025, 1, 31), Status = StatusZadatka.UToku,    ProjekatId = projekti[1].Id },
                new() { Naziv = "Push notifikacije",           Opis = "Firebase Cloud Messaging setup.",           Rok = new DateTime(2025, 2, 28), Status = StatusZadatka.Otvoren,  ProjekatId = projekti[1].Id },

                // Projekat 2 — E-Commerce Portal (planiran)
                new() { Naziv = "Definisanje arhitekture",    Opis = "Odabir stack-a i infrastrukture.",          Rok = new DateTime(2024, 11, 15), Status = StatusZadatka.UToku,   ProjekatId = projekti[2].Id },
                new() { Naziv = "Postavljanje CI/CD",         Opis = "GitHub Actions + Azure deploy pipeline.",   Rok = new DateTime(2024, 12, 31), Status = StatusZadatka.Otvoren, ProjekatId = projekti[2].Id },
            };
            context.Zadaci.AddRange(zadaci);
            context.SaveChanges();

            // ── Angazovanja ───────────────────────────────────────────
            var angazovanja = new List<Angazovanje>
            {
                new() { ProjekatId = projekti[0].Id, ZaposleniId = zaposleni[2].Id, Uloga = "Lead",  Satnica = 65.00m, DatumOd = new DateTime(2024, 1, 15) },
                new() { ProjekatId = projekti[0].Id, ZaposleniId = zaposleni[0].Id, Uloga = "Dev",   Satnica = 50.00m, DatumOd = new DateTime(2024, 1, 15) },
                new() { ProjekatId = projekti[1].Id, ZaposleniId = zaposleni[2].Id, Uloga = "Lead",  Satnica = 70.00m, DatumOd = new DateTime(2024, 6,  1) },
                new() { ProjekatId = projekti[1].Id, ZaposleniId = zaposleni[0].Id, Uloga = "Dev",   Satnica = 55.00m, DatumOd = new DateTime(2024, 6,  1) },
                new() { ProjekatId = projekti[1].Id, ZaposleniId = zaposleni[1].Id, Uloga = "QA",    Satnica = 45.00m, DatumOd = new DateTime(2024, 8,  1) },
            };
            context.Angazovanja.AddRange(angazovanja);
            context.SaveChanges();
        }
    }
}
