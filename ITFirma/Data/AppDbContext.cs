using ITFirma.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITFirma.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Klijent> Klijenti { get; set; }
        public DbSet<Projekat> Projekti { get; set; }
        public DbSet<Zaposleni> Zaposleni { get; set; }
        public DbSet<Zadatak> Zadaci { get; set; }
        public DbSet<Angazovanje> Angazovanja { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite PK za Angazovanje (m2m join klasa)
            modelBuilder.Entity<Angazovanje>()
                .HasKey(a => new { a.ProjekatId, a.ZaposleniId });

            // Angazovanje → Projekat
            modelBuilder.Entity<Angazovanje>()
                .HasOne(a => a.Projekat)
                .WithMany(p => p.Angazovanja)
                .HasForeignKey(a => a.ProjekatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Angazovanje → Zaposleni
            modelBuilder.Entity<Angazovanje>()
                .HasOne(a => a.Zaposleni)
                .WithMany(z => z.Angazovanja)
                .HasForeignKey(a => a.ZaposleniId)
                .OnDelete(DeleteBehavior.Cascade);

            // Projekat → Klijent
            modelBuilder.Entity<Projekat>()
                .HasOne(p => p.Klijent)
                .WithMany(k => k.Projekti)
                .HasForeignKey(p => p.KlijentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Zadatak → Projekat
            modelBuilder.Entity<Zadatak>()
                .HasOne(z => z.Projekat)
                .WithMany(p => p.Zadaci)
                .HasForeignKey(z => z.ProjekatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
